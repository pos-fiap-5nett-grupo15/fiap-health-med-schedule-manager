using System.ComponentModel.DataAnnotations;

namespace Fiap.Health.Med.Schedule.Manager.Application.DTOs.Patient
{
    public class PatientScheduleRequestDto
    {
        [Required(ErrorMessage = "Identificador do agendamento é obrigatório")]
        public long ScheduleId { get; set; }

        [Required(ErrorMessage = "Identificador do paciente é obrigatório")]
        public int PatientId { get; set; }
    }
}
