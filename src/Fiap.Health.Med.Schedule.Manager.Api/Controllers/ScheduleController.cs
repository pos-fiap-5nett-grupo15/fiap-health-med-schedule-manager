using Fiap.Health.Med.Schedule.Manager.Application.Common;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.Doctor.UpdateSchedule;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var result = await this.ScheduleService.GetByScheduleId(id, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Data);
        else
            return StatusCode((int)result.StatusCode, result.Errors);
    }

    [HttpGet("doctor/{doctorId}")]
    public async Task<IActionResult> GetByDoctorAsync(int doctorId, CancellationToken cancellationToken)
    {
        var result = await this.ScheduleService.GetByDoctorId(doctorId, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Data);
        else
            return StatusCode((int)result.StatusCode, result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Domain.Models.Schedule schedule, CancellationToken cancellationToken)
    {
        var result = await this.ScheduleService.RequestCreateScheduleAsync(schedule, cancellationToken);
        return StatusCode((int)result.StatusCode, result.Errors);
    }

    [HttpPatch("{scheduleId}/decline/{doctorId}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeclineScheduleAsync(
        [FromRoute] long scheduleId,
        [FromRoute] int doctorId,
        CancellationToken ct)
    {
        var result = await this.ScheduleService.DeclineScheduleAsync(scheduleId, doctorId, ct);

        if (result.IsSuccess)
            return Ok(result);

        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("{scheduleId}/{doctorId}")]
    public async Task<IActionResult> UpdateSchedule(int scheduleId, int doctorId, [FromBody] UpdateScheduleRequestDto updateSchedule, CancellationToken cancellationToken)
    {
        try
        {
            updateSchedule.ScheduleId = scheduleId;
            updateSchedule.DoctorId = doctorId;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await this.ScheduleService.UpdateScheduleAsync(updateSchedule, cancellationToken);
            if (result.IsSuccess)
                return Ok();
            else
                return StatusCode((int)result.StatusCode, result.Errors);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpPatch("accept/{scheduleId}/{doctorId}")]
    public async Task<IActionResult> AcceptScheduleAsync(
        [FromRoute] long scheduleId,
        [FromRoute] int doctorId,
        CancellationToken ct)
    {
        var result = await this.ScheduleService.AcceptScheduleAsync(scheduleId, doctorId, ct);

        return StatusCode((int)result.StatusCode, result.Errors);
    }
}