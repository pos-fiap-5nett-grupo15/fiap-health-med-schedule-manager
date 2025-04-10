using Fiap.Health.Med.Schedule.Manager.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        var result = await this.ScheduleService.GetAsync(cancellationToken);
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

        return StatusCode((int)result.StatusCode, result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSchedule(int id, [FromBody] Application.DTOs.UpdateSchedule.UpdateScheduleRequestDto updateSchedule, CancellationToken cancellationToken)
    {
        try
        {
            if (updateSchedule is null)
                return BadRequest("Dados vazios não são permitidos");
            else
                updateSchedule.Id = id;

            var result = await this.ScheduleService.UpdateScheduleAsync(updateSchedule, cancellationToken);
            if (result.IsSuccess)
                return Ok();
            else
                return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}