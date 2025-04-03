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

    public bool IsOverlappedBy(Schedule dbModel)
    {
        if(dbModel == null) throw new ArgumentNullException(nameof(dbModel));


        if(
            (
                this.ScheduleTime <= dbModel.ScheduleTime && 
                dbModel.ScheduleTime <= this.ScheduleTime + new TimeSpan(1, 0, 0)
            )
            ||
            (
                this.ScheduleTime <= dbModel.ScheduleTime + new TimeSpan(1, 0, 0) && 
                dbModel.ScheduleTime + new TimeSpan(1, 0, 0) <= this.ScheduleTime + new TimeSpan(1, 0, 0)
            )
        ) return true;
        return false;
    }
}