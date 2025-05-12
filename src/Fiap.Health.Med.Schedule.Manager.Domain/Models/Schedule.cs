using Fiap.Health.Med.Schedule.Manager.Domain.Enum;

namespace Fiap.Health.Med.Schedule.Manager.Domain.Models;

public class Schedule
{
    public long Id { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public float Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime ScheduleTime { get; set; }
    public EScheduleStatus Status { get; set; }
    public string CancelReason { get; set; } = string.Empty;

    public bool IsOverlappedBy(Schedule dbModel)
    {
        if (dbModel == null) throw new ArgumentNullException(nameof(dbModel));

        if (dbModel.Id == this.Id) return false;

        if (
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