using Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;
using Fiap.Health.Med.Schedule.Manager.Infrastructure.UnitOfWork;
using Dapper;

namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly IHealthDatabase _database;


    public ScheduleRepository(IHealthDatabase database)
    {
        this._database = database;
    }

    public Task CreateScheduleAsync(Domain.Models.Schedule schedule, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Domain.Models.Schedule>> GetAsync(CancellationToken cancellationToken)
    {
        var query = @"SELECT * FROM Schedule.Schedule";

        return await _database.Connection.QueryAsync<Domain.Models.Schedule>(query);
    }

    public Task<IEnumerable<Domain.Models.Schedule>> GetScheduleByDoctorIdAsync(int doctorId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
                        {nameof(Domain.Models.Schedule.IsActive)} = @IsActive
                        {nameof(Domain.Models.Schedule.ScheduleTime)} = @ScheduleTime
                       WHERE Id = @Id";
        return await _database.Connection.ExecuteAsync(query, schedule);
    }
}