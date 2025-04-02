using Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;

namespace Fiap.Health.Med.Schedule.Manager.Application.Services;

public class ScheduleService : IScheduleService
{
    public IUnitOfWork UnitOfWork { get; set; }
    
    
    public ScheduleService(IUnitOfWork unitOfWork)
    {
        this.UnitOfWork = unitOfWork;
    }



    public async Task<IEnumerable<Domain.Models.Schedule>> GetAsync(CancellationToken cancellationToken)
    {
        return await this.UnitOfWork.ScheduleRepository.GetAsync(cancellationToken);
    }

    public Task CreateSchedule(Domain.Models.Schedule schedule, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}