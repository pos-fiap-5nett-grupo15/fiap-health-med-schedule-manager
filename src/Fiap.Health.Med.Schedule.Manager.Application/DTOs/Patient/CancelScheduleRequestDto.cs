using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fiap.Health.Med.Schedule.Manager.Application.DTOs.Patient
{
    public class CancelScheduleRequestDto
    {
        [JsonIgnore]
        [Required(ErrorMessage = "O id do agendamento é obrigatório")]
        public long ScheduleId { get; set; }

        [JsonIgnore]
        [Required(ErrorMessage = "O id do paciente é obrigatório")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "O motivo do cancelamento é obrigatório")]
        [MinLength(10, ErrorMessage = "O motivo do cancelamento deve ter no mínimo 10 caracteres")]
        public required string Reason { get; set; }
    }
}
