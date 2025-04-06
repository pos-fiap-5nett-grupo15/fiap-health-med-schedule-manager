using Fiap.Health.Med.Schedule.Manager.Application.Common;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.UpdateSchedule;
using Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;
using FluentValidation;

namespace Fiap.Health.Med.Schedule.Manager.Application.Services;

public class ScheduleService : IScheduleService
{
    public IUnitOfWork UnitOfWork { get; set; }
    private readonly IValidator<UpdateScheduleRequestDto> _updateScheduleRequestValidator;


    public ScheduleService(
        IUnitOfWork unitOfWork,
        IValidator<UpdateScheduleRequestDto> updateScheduleRequestValidator)
    {
        this.UnitOfWork = unitOfWork;
        this._updateScheduleRequestValidator = updateScheduleRequestValidator;
    }



    public async Task<IEnumerable<Domain.Models.Schedule>> GetAsync(CancellationToken cancellationToken)
    {
        return await this.UnitOfWork.ScheduleRepository.GetAsync(cancellationToken);
    }

    public async Task CreateScheduleAsync(Domain.Models.Schedule schedule, CancellationToken cancellationToken)
    {
        if (this.IsPassedTime(schedule, DateTime.Now) || this.IsPassedTime(schedule, DateTime.Now))
            throw new InvalidOperationException();

        var dbModels = await this.GetScheduleByAsync(schedule.DoctorId, schedule.ScheduleTime, cancellationToken);

        var hasAnyOverlap = dbModels.Select(x => schedule.IsOverlappedBy(x)).Any(x => x == true);
        if (hasAnyOverlap)
            throw new InvalidOperationException();

        await this.UnitOfWork.ScheduleRepository.CreateScheduleAsync(schedule, cancellationToken);
    }

    private bool IsPassedTime(Domain.Models.Schedule schedule, DateTime reference)
        => schedule.ScheduleTime <= reference;

    private async Task<IEnumerable<Domain.Models.Schedule>> GetScheduleByAsync(int doctorId, DateTime scheduleTime, CancellationToken cancellationToken)
    {
        return await this.UnitOfWork.ScheduleRepository.GetScheduleByDoctorIdAsync(doctorId, cancellationToken);
    }
    private async Task<Domain.Models.Schedule?> GetSchedulesByIdAsync(long scheduleId, CancellationToken cancellationToken)
    {
        return await this.UnitOfWork.ScheduleRepository.GetScheduleByIdAsync(scheduleId, cancellationToken);
    }

    public async Task<Result> UpdateScheduleAsync(UpdateScheduleRequestDto updateScheduleData, CancellationToken cancellationToken)
    {
        if (await _updateScheduleRequestValidator.ValidateAsync(updateScheduleData) is var validation && !validation.IsValid)
            return Result.Fail(validation.Errors.Select(e => e.ErrorMessage).ToList());

        var foundSchedule = await this.GetSchedulesByIdAsync(updateScheduleData.Id, cancellationToken);

        if (foundSchedule is null)
            return Result.Fail("Agendamento não encontrado");

        var doctorSchedules = await this.GetScheduleByAsync(foundSchedule.DoctorId, foundSchedule.ScheduleTime, cancellationToken);

        foundSchedule.IsActive = updateScheduleData.IsActive;
        foundSchedule.ScheduleTime = updateScheduleData.ScheduleTime;

        var hasAnyOverlap = doctorSchedules.Select(x => foundSchedule.IsOverlappedBy(x)).Any(x => x);
        if (hasAnyOverlap)
            return Result.Fail("Data do agendamento em conflito com data(s) existente(s)");

        if (await this.UnitOfWork.ScheduleRepository.UpdateScheduleAsync(foundSchedule, cancellationToken) > 0)
            return Result.Ok();
        else
            throw new Exception("Erro ao atualizar o agendamento");
    }
}