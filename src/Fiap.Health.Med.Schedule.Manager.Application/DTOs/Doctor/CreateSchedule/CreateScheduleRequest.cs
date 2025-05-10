using System.ComponentModel.DataAnnotations;

namespace Fiap.Health.Med.Schedule.Manager.Application.DTOs.Doctor.CreateSchedule
{
    public class CreateScheduleRequest
    {
        [Required(ErrorMessage = "Id do médico é obrigatório")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Data do agendamento é obrigatória")]
        public DateTime ScheduleTime { get; set; }

        [Required(ErrorMessage = "Preço do agendamento é obrigatório")]
        [Range(0.01, 10000.00, ErrorMessage = "O preço deve ser maior que zero e menor que 10.000")]
        public float Price { get; set; }
    }
}
