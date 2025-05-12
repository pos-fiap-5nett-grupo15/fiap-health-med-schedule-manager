namespace Fiap.Health.Med.Schedule.Manager.Application.Services.QueueMessages;

public class CreateScheduleMessage
{
    public int Id { get; set; }

    public CreateScheduleMessage(int id)
    {
        Id = id;
        
    }
}