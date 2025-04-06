using Fiap.Health.Med.Schedule.Manager.Application.DTOs.UpdateSchedule;
using FluentValidation;

namespace Fiap.Health.Med.Schedule.Manager.Application.Validators.UpdateSchedule
{
    public class UpdateScheduleValidator : AbstractValidator<UpdateScheduleRequestDto>
    {
        public UpdateScheduleValidator()
        {
            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("Flag de agendamento ativo é obrigatória");
            RuleFor(x => x.ScheduleTime)
                .NotNull()
                .WithMessage("Data de agendamento é obrigatória")
                .Must(x => x >= DateTime.Now)
                .WithMessage("Data do agendamento deve ser maior que a data atual");
        }
    }
}
