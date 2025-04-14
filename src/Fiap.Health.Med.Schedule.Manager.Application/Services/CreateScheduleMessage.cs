namespace Fiap.Health.Med.Schedule.Manager.Application.Services;

public class CreateScheduleMessage
{
    public int Id { get; set; }

    public CreateScheduleMessage(int id)
    {
        this.Id = id;
        
    }
}