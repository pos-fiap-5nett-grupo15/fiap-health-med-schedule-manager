using Fiap.Health.Med.Schedule.Manager.Application.Common;
using Fiap.Health.Med.Schedule.Manager.Domain.Enum;
using Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;
using System.Net;

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

    public async Task CreateScheduleAsync(Domain.Models.Schedule schedule, CancellationToken cancellationToken)
    {
        if (IsPassedTime(schedule, DateTime.Now) || IsPassedTime(schedule, DateTime.Now))
            throw new InvalidOperationException();

        var dbModels = await this.GetScheduleByAsync(schedule.DoctorId, schedule.ScheduleTime, cancellationToken);

        var hasAnyOverlap = dbModels.Select(x => schedule.IsOverlappedBy(x)).Any(x => x == true);
        if (hasAnyOverlap)
            throw new InvalidOperationException();

        await this.UnitOfWork.ScheduleRepository.CreateScheduleAsync(schedule, cancellationToken);
    }

    public async Task<Result> RefuseScheduleAsync(long scheduleId, int doctorId, CancellationToken ct)
    {
        (var schedule, var getScheduleError) = await this.UnitOfWork.ScheduleRepository.GetScheduleByIdAndDoctorIdAsync(scheduleId, doctorId, ct);
        if (!string.IsNullOrWhiteSpace(getScheduleError))
            return Result.Fail(HttpStatusCode.UnprocessableContent, getScheduleError);

        if (schedule is null)
            return Result.Fail(HttpStatusCode.NotFound, "Agendamento não encontrado.");

        if (await this.UnitOfWork.ScheduleRepository.UpdatescheduleStatusAsync(scheduleId, EScheduleStatus.REFUSED, ct) is (var success, var updateError) && !success)
            return Result.Fail(HttpStatusCode.UnprocessableContent, !string.IsNullOrWhiteSpace(updateError) ? updateError : "Um erro ocorreu ao atualizar status do Agendamento.");
        
        return Result.Success(HttpStatusCode.NoContent);
    }

    #region Private methods:
    private static bool IsPassedTime(Domain.Models.Schedule schedule, DateTime reference)
        => schedule.ScheduleTime <= reference;

    private async Task<IEnumerable<Domain.Models.Schedule>> GetScheduleByAsync(int doctorId, DateTime scheduleTime, CancellationToken cancellationToken)
    {
        return await this.UnitOfWork.ScheduleRepository.GetScheduleByDoctorIdAsync(doctorId, cancellationToken);
    }
    #endregion Private methods.
}