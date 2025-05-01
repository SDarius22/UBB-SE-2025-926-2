using Backend.DatabaseServices;
using Backend.DatabaseServices.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleDatabaseService _scheduleService;

        public ScheduleController(IScheduleDatabaseService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // GET: api/schedule
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduleModel>>> GetSchedules()
        {
            try
            {
                var schedules = await _scheduleService.GetSchedules();
                if (schedules == null)
                    return NotFound("No schedules found");

                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving schedules: {ex.Message}");
            }
        }

        // POST: api/schedule
        [HttpPost]
        public async Task<ActionResult<bool>> AddSchedule([FromBody] ScheduleModel schedule)
        {
            try
            {
                if (!await _scheduleService.DoesDoctorExist(schedule.DoctorId))
                    return BadRequest("Doctor does not exist");

                if (!await _scheduleService.DoesShiftExist(schedule.ShiftId))
                    return BadRequest("Shift does not exist");

                var success = await _scheduleService.AddSchedule(schedule);
                if (!success)
                    return BadRequest("Failed to add schedule");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding schedule: {ex.Message}");
            }
        }

        // PUT: api/schedule/5
        [HttpPut("{scheduleId}")]
        public async Task<ActionResult<bool>> UpdateSchedule(int scheduleId, [FromBody] ScheduleModel schedule)
        {
            try
            {
                if (scheduleId != schedule.ScheduleId)
                    return BadRequest("Schedule ID mismatch");

                if (!await _scheduleService.DoesScheduleExist(scheduleId))
                    return NotFound($"Schedule with ID {scheduleId} not found");

                if (!await _scheduleService.DoesDoctorExist(schedule.DoctorId))
                    return BadRequest("Doctor does not exist");

                if (!await _scheduleService.DoesShiftExist(schedule.ShiftId))
                    return BadRequest("Shift does not exist");

                var success = await _scheduleService.UpdateSchedule(schedule);
                if (!success)
                    return BadRequest("Failed to update schedule");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating schedule: {ex.Message}");
            }
        }

        // DELETE: api/schedule/5
        [HttpDelete("{scheduleId}")]
        public async Task<ActionResult<bool>> DeleteSchedule(int scheduleId)
        {
            try
            {
                if (!await _scheduleService.DoesScheduleExist(scheduleId))
                    return NotFound($"Schedule with ID {scheduleId} not found");

                var success = await _scheduleService.DeleteSchedule(scheduleId);
                if (!success)
                    return BadRequest("Failed to delete schedule");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting schedule: {ex.Message}");
            }
        }
    }
}
