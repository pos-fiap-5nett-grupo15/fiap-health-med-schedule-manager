using Fiap.Health.Med.Schedule.Manager.Application.Common;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.UpdateSchedule;
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

    public async Task<Result> AcceptScheduleAsync(long scheduleId, int doctorId, CancellationToken ct)
    {
        (var schedule, var getScheduleError) = await this.UnitOfWork.ScheduleRepository.GetScheduleByIdAndDoctorIdAsync(scheduleId, doctorId, ct);
        if (!string.IsNullOrWhiteSpace(getScheduleError))
            return Result.Fail(HttpStatusCode.UnprocessableContent, getScheduleError);

        if (schedule is null)
            return Result.Fail(HttpStatusCode.NotFound, "Agendamento não encontrado.");

        if (schedule.Status == EScheduleStatus.PENDING_CONFIRMATION)
        {
            if (schedule.ScheduleTime.Date < DateTime.Now.Date)
            {
                if (await this.UnitOfWork.ScheduleRepository.DeleteScheduleStatusAsync(scheduleId, ct) is (var successDelete, var updateDeleteError) && !successDelete)
                    return Result.Fail(HttpStatusCode.UnprocessableContent, !string.IsNullOrWhiteSpace(updateDeleteError) ? updateDeleteError : "Um erro ocorreu ao excluir o Agendamento.");
            }
            else
            {
                if (await this.UnitOfWork.ScheduleRepository.UpdatescheduleStatusAsync(scheduleId, EScheduleStatus.CONFIRMED, ct) is (var successConfirmed, var updateConfirmedError) && !successConfirmed)
                    return Result.Fail(HttpStatusCode.UnprocessableContent, !string.IsNullOrWhiteSpace(updateConfirmedError) ? updateConfirmedError : "Um erro ocorreu ao atualizar status para confirmar o Agendamento.");
            }

        }
        else if (schedule.Status == EScheduleStatus.CANCELED_BY_DOCTOR)
        {
            if (await this.UnitOfWork.ScheduleRepository.UpdatescheduleStatusAsync(scheduleId, EScheduleStatus.REFUSED, ct) is (var successRefused, var updateRefusedError) && !successRefused)
                return Result.Fail(HttpStatusCode.UnprocessableContent, !string.IsNullOrWhiteSpace(updateRefusedError) ? updateRefusedError : "Um erro ocorreu ao atualizar status para cancelar o Agendamento.");
        }
        else
        {
            return Result.Fail(HttpStatusCode.UnprocessableContent, "Não é possível aceitar o Agendamento");
        }

        return Result.Success(HttpStatusCode.NoContent);
    }

    public async Task<Result> UpdateScheduleAsync(UpdateScheduleRequestDto updateScheduleData, CancellationToken cancellationToken)
    {
        if (updateScheduleData.ScheduleTime <= DateTime.Now)
            return Result.Fail(HttpStatusCode.BadRequest, "Data do agendamento deve ser maior que a data atual");

        var foundSchedule = await this.GetScheduleByIdAsync(updateScheduleData.Id, cancellationToken);

        if (foundSchedule is null)
            return Result.Fail(HttpStatusCode.NotFound, "Agendamento não encontrado");

        var doctorSchedules = await this.GetScheduleByAsync(foundSchedule.DoctorId, foundSchedule.ScheduleTime, cancellationToken);
        foundSchedule.ScheduleTime = updateScheduleData.ScheduleTime;
        foundSchedule.Status = new EScheduleStatus[] { EScheduleStatus.PENDING_CONFIRMATION, EScheduleStatus.CONFIRMED }.Contains(foundSchedule.Status) ? EScheduleStatus.PENDING_CONFIRMATION : EScheduleStatus.AVAILABLE;

        if (doctorSchedules is not null)
        {
            var hasAnyOverlap = doctorSchedules.Select(x => foundSchedule.IsOverlappedBy(x)).Any(x => x);
            if (hasAnyOverlap)
                return Result.Fail(HttpStatusCode.BadRequest, "Data do agendamento em conflito com data(s) existente(s)");
        }

        if (await this.UnitOfWork.ScheduleRepository.UpdateScheduleAsync(foundSchedule, cancellationToken) > 0)
            return Result.Success(HttpStatusCode.OK);
        else
            return Result.Fail(HttpStatusCode.InternalServerError, "Erro ao atualizar o agendamento");
    }

    #region Private methods:
    private static bool IsPassedTime(Domain.Models.Schedule schedule, DateTime reference)
        => schedule.ScheduleTime <= reference;
    private async Task<IEnumerable<Domain.Models.Schedule>> GetScheduleByAsync(int doctorId, DateTime scheduleTime, CancellationToken cancellationToken)
    {
        return await this.UnitOfWork.ScheduleRepository.GetScheduleByDoctorIdAsync(doctorId, cancellationToken);
    }
    private async Task<Domain.Models.Schedule?> GetScheduleByIdAsync(long scheduleId, CancellationToken cancellationToken)
    {
        return await this.UnitOfWork.ScheduleRepository.GetScheduleByIdAsync(scheduleId, cancellationToken);
    }
    #endregion Private methods.
}