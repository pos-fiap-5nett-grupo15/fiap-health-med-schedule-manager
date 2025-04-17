using Fiap.Health.Med.Schedule.Manager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Health.Med.Schedule.Manager.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScheduleController : ControllerBase
{
    public IScheduleService ScheduleService { get; set; }
    
    public ScheduleController(IScheduleService scheduleService)
    {
        this.ScheduleService = scheduleService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    { 
        var result =  await this.ScheduleService.GetAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Domain.Models.Schedule schedule, CancellationToken cancellationToken)
    {
        await this.ScheduleService.CreateScheduleAsync(schedule, cancellationToken);
        return Ok();
    }

    [HttpPatch("refuse/{scheduleId}/{doctorId}")]
    public async Task<IActionResult> RefuseScheduleAsync(
        [FromRoute] long scheduleId,
        [FromRoute] int doctorId,
        CancellationToken ct)
    {
        var result = await this.ScheduleService.RefuseScheduleAsync(scheduleId, doctorId, ct);

        return StatusCode((int) result.StatusCode, result.Errors);
    }

    [HttpPatch("{scheduleId}/accept/{doctorId}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AcceptScheduleAsync(
        [FromRoute] long scheduleId,
        [FromRoute] int doctorId,
        CancellationToken ct)
    {
        var result = await this.ScheduleService.AcceptScheduleAsync(scheduleId, doctorId, ct);

        return StatusCode((int)result.StatusCode, result.Errors);
    }
}