namespace Fiap.Health.Med.Schedule.Manager.Application.Services.QueueMessages
{
    public class PatientCancelScheduleMessage(long scheduleId, string cancelReason)
    {
        public long ScheduleId { get; set; } = scheduleId;
        public string CancelReason { get; set; } = cancelReason;
    }
}
