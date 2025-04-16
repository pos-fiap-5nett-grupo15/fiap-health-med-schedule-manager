using Fiap.Health.Med.Schedule.Manager.Application.DTOs.Patient;
using Fiap.Health.Med.Schedule.Manager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Health.Med.Schedule.Manager.Api.Controllers
{
    [Route("api/Patient")]
    [ApiController()]
    public class PatientSchedule : ControllerBase
    {
        private IScheduleService ScheduleService { get; set; }

        public PatientSchedule(IScheduleService patientScheduleService)
        {
            this.ScheduleService = patientScheduleService;
        }

        [HttpPost]
        public async Task<IActionResult> RequestScheduleAsync([FromBody] PatientScheduleRequestDto patientScheduleData, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await this.ScheduleService.ScheduleToPatientAsync(patientScheduleData, cancellationToken);

            if (result.IsSuccess)
                return Created();
            else
                return StatusCode((int)result.StatusCode, result.Errors);
        }

        [HttpPatch("cancel/{scheduleId}/{patientId}")]
        public async Task<IActionResult> CancelAsync(long scheduleId, int patientId, [FromBody] CancelScheduleRequestDto patientCancelData, CancellationToken cancellationToken)
        {
            patientCancelData.ScheduleId = scheduleId;
            patientCancelData.PatientId = patientId;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await this.ScheduleService.CancelScheduleAsync(patientCancelData, cancellationToken);

            if (result.IsSuccess)
                return NoContent();
            else
                return StatusCode((int)result.StatusCode, result.Errors);
        }

        [HttpGet("{patientId}")]
        public async Task<IActionResult> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken)
        {
            var result = await this.ScheduleService.GetByPatientId(patientId, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return StatusCode((int)result.StatusCode, result.Errors);
        }
    }
}
