using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fiap.Health.Med.Schedule.Manager.Application.DTOs.Doctor.UpdateSchedule
{
    public class UpdateScheduleRequestDto
    {
        [JsonIgnore]
        [Required(ErrorMessage = "Id do agendamento é obrigatório")]
        public long ScheduleId { get; set; }

        [JsonIgnore]
        [Required(ErrorMessage = "Id do médico é obrigatório")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Data do agendamento é obrigatória")]
        public DateTime ScheduleTime { get; set; }
    }
}
