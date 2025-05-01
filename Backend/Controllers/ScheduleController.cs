using Backend.DatabaseServices;
using Backend.DatabaseServices.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System;
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
                if (schedules == null || schedules.Count == 0)
                    return NotFound("No schedules found");

                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving schedules: {ex.Message}");
            }
        }

        // GET: api/schedule/doctor/{doctorId}
        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<ScheduleModel>>> GetSchedulesByDoctor(int doctorId)
        {
            try
            {
                var schedules = await _scheduleService.GetSchedulesByDoctor(doctorId);
                if (schedules == null || schedules.Count == 0)
                    return NotFound($"No schedules found for doctor with ID {doctorId}");

                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving schedules for doctor: {ex.Message}");
            }
        }

        // POST: api/schedule
        [HttpPost]
        public async Task<ActionResult<bool>> AddSchedule([FromBody] ScheduleModel schedule)
        {
            try
            {
                // Check if the doctor exists
                if (!await _scheduleService.DoesDoctorExist(schedule.DoctorId))
                    return BadRequest("Doctor does not exist");

                // Check if the shift exists
                if (!await _scheduleService.DoesShiftExist(schedule.ShiftId))
                    return BadRequest("Shift does not exist");

                // Add the schedule
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
                // Check if the schedule ID matches
                if (scheduleId != schedule.ScheduleId)
                    return BadRequest("Schedule ID mismatch");

                // Check if the schedule exists
                if (!await _scheduleService.DoesScheduleExist(scheduleId))
                    return NotFound($"Schedule with ID {scheduleId} not found");

                // Check if the doctor exists
                if (!await _scheduleService.DoesDoctorExist(schedule.DoctorId))
                    return BadRequest("Doctor does not exist");

                // Check if the shift exists
                if (!await _scheduleService.DoesShiftExist(schedule.ShiftId))
                    return BadRequest("Shift does not exist");

                // Update the schedule
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
                // Check if the schedule exists
                if (!await _scheduleService.DoesScheduleExist(scheduleId))
                    return NotFound($"Schedule with ID {scheduleId} not found");

                // Delete the schedule
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

        // Check if the schedule exists
        [HttpGet("exists/{scheduleId}")]
        public async Task<ActionResult<bool>> DoesScheduleExist(int scheduleId)
        {
            try
            {
                var exists = await _scheduleService.DoesScheduleExist(scheduleId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error checking if schedule exists: {ex.Message}");
            }
        }

        // Check if the doctor exists
        [HttpGet("doctor-exists/{doctorId}")]
        public async Task<ActionResult<bool>> DoesDoctorExist(int doctorId)
        {
            try
            {
                var exists = await _scheduleService.DoesDoctorExist(doctorId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error checking if doctor exists: {ex.Message}");
            }
        }

        // Check if the shift exists
        [HttpGet("shift-exists/{shiftId}")]
        public async Task<ActionResult<bool>> DoesShiftExist(int shiftId)
        {
            try
            {
                var exists = await _scheduleService.DoesShiftExist(shiftId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error checking if shift exists: {ex.Message}");
            }
        }
    }
}
