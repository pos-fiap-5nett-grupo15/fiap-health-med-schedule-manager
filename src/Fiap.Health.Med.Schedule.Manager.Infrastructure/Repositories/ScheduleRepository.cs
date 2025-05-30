using Dapper;
using Fiap.Health.Med.Schedule.Manager.Domain.Enum;
using Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;
using Fiap.Health.Med.Schedule.Manager.Infrastructure.UnitOfWork;
using System.Numerics;
using System.Xml.XPath;

namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly IHealthDatabase _database;

    public ScheduleRepository(IHealthDatabase database)
    {
        this._database = database;
    }

    public async Task<bool> CreateScheduleAsync(Domain.Models.Schedule schedule, CancellationToken cancellationToken)
    {
        var query = @"INSERT INTO Schedule.Schedule (DoctorId, PatientId, CreatedAt, UpdatedAt, ScheduleTime, Status)
                      VALUES (@DoctorId, @PatientId, @CreatedAt, @UpdatedAt, @ScheduleTime, @Status)";

        return (await _database.Connection.ExecuteScalarAsync<int>(query, schedule)) > 0;
    }

    public async Task<IEnumerable<Domain.Models.Schedule>> GetAsync(CancellationToken cancellationToken)
    {
        var query = @"SELECT * FROM Schedule.Schedule";

        return await _database.Connection.QueryAsync<Domain.Models.Schedule>(query, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Models.Schedule>> GetScheduleByDoctorIdAsync(int doctorId, CancellationToken cancellationToken)
    {
        var query = @"SELECT * FROM Schedule.Schedule WHERE DoctorId = @DoctorId";
        var result = await this._database.Connection.QueryAsync<Domain.Models.Schedule>(query, new { DoctorId = doctorId });
        return result;
    }


    public async Task<(Domain.Models.Schedule?, string)> GetScheduleByIdAndDoctorIdAsync(long scheduleId, int doctorId, CancellationToken ct)
    {
        try
        {
            var query = @$"SELECT
                                *
                            FROM Schedule.Schedule 
                            WHERE Id = {scheduleId}
                            AND {nameof(Domain.Models.Schedule.DoctorId)} = {doctorId}";

            return (await _database.Connection.QueryFirstOrDefaultAsync<Domain.Models.Schedule>(query, ct), string.Empty);
        }
        catch (Exception e)
        {

            return (null, e.Message);
        }
    }

    public async Task<(bool, string)> UpdatescheduleStatusAsync(long scheduleId, EScheduleStatus newStatus, CancellationToken ct)
    {
        try
        {
            var query = @$"UPDATE Schedule.Schedule 
                      SET {nameof(Domain.Models.Schedule.Status)} = {(int)newStatus}
                      WHERE Id = {scheduleId}";

            return (await _database.Connection.ExecuteAsync(query) > 0, string.Empty);
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }
    public async Task<int> CreatePendingScheduleAsync(Domain.Models.Schedule schedule, CancellationToken cancellationToken)
    {
        var query = @"INSERT INTO Schedule.Schedule (DoctorId, PatientId, CreatedAt, UpdatedAt, ScheduleTime, Status, Price)
                      VALUES (@DoctorId, @PatientId, @CreatedAt, @UpdatedAt, @ScheduleTime, @Status, @Price);
                      SELECT SCOPE_IDENTITY();";

        return (await _database.Connection.ExecuteScalarAsync<int>(query, schedule));
    }

    public async Task<Domain.Models.Schedule> GetScheduleByIdAsync(int scheduleId, CancellationToken ct)
    {
        var query = @"SELECT * FROM Schedule.Schedule WHERE Id = @ScheduleId";
        var param = new { ScheduleId = scheduleId };

        var result =
            await _database.Connection.QueryAsync<Domain.Models.Schedule>(query,
                param: new { ScheduleId = scheduleId });

        return result.FirstOrDefault();
    }
    public async Task<(bool, string)> DeleteScheduleStatusAsync(long scheduleId, CancellationToken ct)
    {
        try
        {
            var query = @$"DELETE FROM Schedule.Schedule 
                      WHERE Id = {scheduleId}";

            return (await _database.Connection.ExecuteAsync(query) > 0, string.Empty);
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }

    public async Task<Domain.Models.Schedule?> GetScheduleByIdAsync(long scheduleId, CancellationToken cancellationToken)
    {
        var query = @"SELECT * FROM Schedule.Schedule WHERE Id = @Id";
        return await _database.Connection.QueryFirstOrDefaultAsync<Domain.Models.Schedule?>(query, new { Id = scheduleId });
    }

    public async Task<int> UpdateScheduleAsync(Domain.Models.Schedule schedule, CancellationToken cancellationToken)
    {
        var query = @$"UPDATE Schedule.Schedule 
                       SET
                        {nameof(Domain.Models.Schedule.Status)} = @Status,
                        {nameof(Domain.Models.Schedule.ScheduleTime)} = @ScheduleTime,
                        {nameof(Domain.Models.Schedule.Price)} = @Price,
                        {nameof(Domain.Models.Schedule.UpdatedAt)} = @UpdatedAt
                       WHERE Id = @Id";
        return await _database.Connection.ExecuteAsync(query, schedule);
    }

    public async Task<int> ScheduleToPatientAsync(Domain.Models.Schedule schedule, CancellationToken cancellationToken)
    {
        var query = @$"UPDATE Schedule.Schedule 
                       SET
                        {nameof(Domain.Models.Schedule.PatientId)} = @PatientId,
                        {nameof(Domain.Models.Schedule.Status)} = @Status,
                        {nameof(Domain.Models.Schedule.UpdatedAt)} = @UpdatedAt
                       WHERE Id = @Id";

        return await _database.Connection.ExecuteAsync(query, schedule);
    }

    public async Task<IEnumerable<Domain.Models.Schedule>> GetSchedulesByPatientIdAsync(int patientId, CancellationToken cancellationToken)
    {
        var query = @$"SELECT * FROM Schedule.Schedule WHERE PatientId = @PatientId";

        return await _database.Connection.QueryAsync<Domain.Models.Schedule>(query, new { PatientId = patientId });
    }

    public async Task<int> CancelScheduleAsync(Domain.Models.Schedule schedule, CancellationToken ct)
    {
        var query = $@"UPDATE Schedule.Schedule 
                       SET
                        {nameof(Domain.Models.Schedule.Status)} = @Status,
                        {nameof(Domain.Models.Schedule.CancelReason)} = @CancelReason,
                        {nameof(Domain.Models.Schedule.UpdatedAt)} = @UpdatedAt
                       WHERE Id = @Id";

        return await _database.Connection.ExecuteAsync(query, schedule);
    }
}