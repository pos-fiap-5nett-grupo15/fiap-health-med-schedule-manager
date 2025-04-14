using System.Net;
using System.Text;
using Fiap.Health.Med.Schedule.Manager.Application.Common;
using Fiap.Health.Med.Schedule.Manager.Domain.Enum;
using Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;
using Fiap.Health.Med.Schedule.Manager.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Fiap.Health.Med.Schedule.Manager.Application.Services;

public class ScheduleService : IScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ScheduleService> _logger;
    private readonly IProducerSettings _producer;
    
    public ScheduleService(
        IUnitOfWork unitOfWork,
        IProducerSettings producer,
        ILogger<ScheduleService> logger)
    {
        this._unitOfWork = unitOfWork;
        this._logger = logger;
        this._producer = producer;
    }

    
    public async Task<IEnumerable<Domain.Models.Schedule>> GetAsync(CancellationToken cancellationToken)
    {
        return await this._unitOfWork.ScheduleRepository.GetAsync(cancellationToken);
    }

    public async Task CreateScheduleAsync(Domain.Models.Schedule schedule, CancellationToken cancellationToken)
    {
        if (IsPassedTime(schedule, DateTime.Now) || IsPassedTime(schedule, DateTime.Now))
            throw new InvalidOperationException();

        var dbModels = await this.GetScheduleByAsync(schedule.DoctorId, schedule.ScheduleTime, cancellationToken);

        var hasAnyOverlap = dbModels.Select(x => schedule.IsOverlappedBy(x)).Any(x => x == true);
        if (hasAnyOverlap)
            throw new InvalidOperationException();

        await this._unitOfWork.ScheduleRepository.CreateScheduleAsync(schedule, cancellationToken);
    }

    public async Task HandleCreateAsync(CreateScheduleMessage? deserialize, CancellationToken cancellationToken)
    {
        if(deserialize == null) throw new NullReferenceException();
        var schedule = await this._unitOfWork.ScheduleRepository.GetScheduleByIdAsync(deserialize.Id, cancellationToken);
        
        if (IsPassedTime(schedule, DateTime.Now) || IsPassedTime(schedule, DateTime.Now))
            throw new InvalidOperationException();

        var dbModels 
            = (await this.GetScheduleByAsync(schedule.DoctorId, schedule.ScheduleTime, cancellationToken)).Where( x => x.Status != EScheduleStatus.UNDEFINED );
        
        var hasAnyOverlap = dbModels.Select(x => schedule.IsOverlappedBy(x)).Any(x => x == true);
        
        var newStatus = hasAnyOverlap ? EScheduleStatus.REFUSED : EScheduleStatus.CONFIRMED;
        
        await this._unitOfWork.ScheduleRepository.UpdatescheduleStatusAsync(schedule.Id,newStatus, cancellationToken);
    }

    public async Task<Result<int>> RequestCreateScheduleAsync(Domain.Models.Schedule schedule, CancellationToken cancellationToken)
    {
        try
        {
            schedule.Status = EScheduleStatus.UNDEFINED;
            var id = await this._unitOfWork.ScheduleRepository.CreatePendingScheduleAsync(schedule, cancellationToken); // TODO Verificar exceções do dapper
            await PublishScheduleAsync(new CreateScheduleMessage(id), this._producer, cancellationToken);
            return Result<int>.Success(HttpStatusCode.Created,id);
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "An error occurring while creating schedule.");
            throw;
        }
        
    }
    

    public static async Task PublishScheduleAsync<T>(T message, IProducerSettings settings,
        CancellationToken cancellationToken)
    {
        ConnectionFactory factory = new()
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password
        };

        using (IConnection connection = await factory.CreateConnectionAsync(cancellationToken))
        using (IChannel channel = await connection.CreateChannelAsync(null, cancellationToken))
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            await channel.BasicPublishAsync(settings.Exchange, settings.RoutingKey,new ReadOnlyMemory<byte>(body), cancellationToken);
        }
    }

    public async Task<Result> RefuseScheduleAsync(long scheduleId, int doctorId, CancellationToken ct)
    {
        (var schedule, var getScheduleError) = await this._unitOfWork.ScheduleRepository.GetScheduleByIdAndDoctorIdAsync(scheduleId, doctorId, ct);
        if (!string.IsNullOrWhiteSpace(getScheduleError))
            return Result.Fail(HttpStatusCode.UnprocessableContent, getScheduleError);

        if (schedule is null)
            return Result.Fail(HttpStatusCode.NotFound, "Agendamento não encontrado.");

        if (await this._unitOfWork.ScheduleRepository.UpdatescheduleStatusAsync(scheduleId, EScheduleStatus.REFUSED, ct) is (var success, var updateError) && !success)
            return Result.Fail(HttpStatusCode.UnprocessableContent, !string.IsNullOrWhiteSpace(updateError) ? updateError : "Um erro ocorreu ao atualizar status do Agendamento.");
        
        return Result.Success(HttpStatusCode.NoContent);
    }

    #region Private methods:
    private static bool IsPassedTime(Domain.Models.Schedule schedule, DateTime reference)
        => schedule.ScheduleTime <= reference;

    private async Task<IEnumerable<Domain.Models.Schedule>> GetScheduleByAsync(int doctorId, DateTime scheduleTime, CancellationToken cancellationToken)
    {
        return await this._unitOfWork.ScheduleRepository.GetScheduleByDoctorIdAsync(doctorId, cancellationToken);
    }
    #endregion Private methods.
}