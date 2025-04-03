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
}