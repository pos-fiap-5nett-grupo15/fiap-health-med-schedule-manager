using System.Text.Json.Serialization;

namespace Fiap.Health.Med.Schedule.Manager.Application.DTOs.UpdateSchedule
{
    public class UpdateScheduleRequestDto
    {
        [JsonIgnore]
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime ScheduleTime { get; set; }
    }
}
