using Fiap.Health.Med.Schedule.Manager.Application.Common;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.Doctor.CreateSchedule;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.Doctor.UpdateSchedule;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.Patient;
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
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await this.ScheduleService.GetAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{scheduleId}")]
    public async Task<IActionResult> GetByIdAsync(long scheduleId, CancellationToken cancellationToken)
    {
        var result = await this.ScheduleService.GetByScheduleId(scheduleId, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Data);
        else
            return StatusCode((int)result.StatusCode, result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleRequest createScheduleRequest, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await this.ScheduleService.RequestCreateScheduleAsync(createScheduleRequest, cancellationToken);
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

    [HttpPatch("{scheduleId}/doctor/{doctorId}/decline")]
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

    [HttpPut("{scheduleId}/doctor/{doctorId}/update")]
    public async Task<IActionResult> UpdateSchedule(
        [FromRoute] int scheduleId,
        [FromRoute] int doctorId,
        [FromBody] UpdateScheduleRequestDto updateSchedule,
        CancellationToken cancellationToken)
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

    [HttpPatch("{scheduleId}/doctor/{doctorId}/accept")]
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


    [HttpPost("{scheduleId}/patient/{patientId}/request-schedule")]
    public async Task<IActionResult> RequestScheduleAsync(
        [FromRoute] int scheduleId,
        [FromRoute] int patientId,
        CancellationToken cancellationToken)
    {
        var patientScheduleData = new PatientScheduleRequestDto
        {
            ScheduleId = scheduleId,
            PatientId = patientId
        };

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await this.ScheduleService.RequestScheduleToPatientAsync(patientScheduleData, cancellationToken);

        if (result.IsSuccess)
            return Created();
        else
            return StatusCode((int)result.StatusCode, result.Errors);
    }

    [HttpPatch("{scheduleId}/patient/{patientId}/cancel")]
    public async Task<IActionResult> CancelAsync(
        [FromRoute] long scheduleId,
        [FromRoute] int patientId,
        [FromBody] CancelScheduleRequestDto patientCancelData,
        CancellationToken cancellationToken)
    {
        patientCancelData.ScheduleId = scheduleId;
        patientCancelData.PatientId = patientId;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await this.ScheduleService.RequestCancelScheduleAsync(patientCancelData, cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        else
            return StatusCode((int)result.StatusCode, result.Errors);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetByPatientAsync(int patientId, CancellationToken cancellationToken)
    {
        var result = await this.ScheduleService.GetByPatientId(patientId, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Data);
        else
            return StatusCode((int)result.StatusCode, result.Errors);
    }
}