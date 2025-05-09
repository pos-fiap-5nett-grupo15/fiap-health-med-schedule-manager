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

        [Required(ErrorMessage = "Preço do agendamento é obrigatório")]
        [Range(0.01, 10000.00, ErrorMessage = "O preço deve ser maior que zero e menor que 10.000")]
        public float Price { get; set; }
    }
}
