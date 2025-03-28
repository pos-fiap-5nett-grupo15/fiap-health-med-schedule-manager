namespace Fiap.Health.Med.Schedule.Manager.Domain.Models;

public class Schedule
{
    public long Id { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime ScheduleTime { get; set; }
    public string DoctorName { get; set; }
    public string PatientName { get; set; }
    public bool IsConfirmed { get; set; }
}