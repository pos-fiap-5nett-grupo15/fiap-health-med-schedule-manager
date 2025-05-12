namespace Fiap.Health.Med.Schedule.Manager.Domain.Enum
{
    public enum EScheduleStatus
    {
        UNDEFINED,
        AVAILABLE,
        PENDING_CONFIRMATION,
        CONFIRMED,
        REFUSED,
        CANCELED_BY_DOCTOR,
        CANCELED_BY_PATIENT
    }
}
