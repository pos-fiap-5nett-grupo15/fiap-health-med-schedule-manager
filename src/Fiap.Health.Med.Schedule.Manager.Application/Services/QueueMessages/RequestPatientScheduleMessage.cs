namespace Fiap.Health.Med.Schedule.Manager.Application.Services.QueueMessages
{
    public class RequestPatientScheduleMessage(long scheduleId, int patientId)
    {
        public long ScheduleId { get; set; } = scheduleId;
        public int PatientId { get; set; } = patientId;
    }
}
