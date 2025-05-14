using Backend.DatabaseServices;
using Backend.DatabaseServices.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftsController : ControllerBase
    {
        private readonly IShiftsDatabaseService _shiftsService;

        public ShiftsController(IShiftsDatabaseService shiftsService)
        {
            _shiftsService = shiftsService;
        }

        // GET: api/shifts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ShiftModel>>> GetShifts()
        {
            try
            {
                var shifts = await _shiftsService.GetShifts();
                if (shifts == null)
                    return NotFound("No shifts found");

                return Ok(shifts);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving shifts: {ex.Message}");
            }
        }

        // GET: api/shifts/doctor/5
        [HttpGet("doctor/{doctorId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ShiftModel>>> GetShiftsByDoctorId(int doctorId)
        {
            try
            {
                var shifts = await _shiftsService.GetShiftsByDoctorId(doctorId);
                if (shifts == null)
                    return NotFound($"No shifts found for doctor ID {doctorId}");

                return Ok(shifts);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving shifts: {ex.Message}");
            }
        }

        // GET: api/shifts/doctor/5/daytime
        [HttpGet("doctor/{doctorId}/daytime")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ShiftModel>>> GetDoctorDaytimeShifts(int doctorId)
        {
            try
            {
                var shifts = await _shiftsService.GetDoctorDaytimeShifts(doctorId);
                if (shifts == null)
                    return NotFound($"No daytime shifts found for doctor ID {doctorId}");

                return Ok(shifts);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving daytime shifts: {ex.Message}");
            }
        }

        // POST: api/shifts
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<bool>> AddShift([FromBody] ShiftModel shift)
        {
            try
            {
                var success = await _shiftsService.AddShift(shift);
                if (!success)
                    return BadRequest("Failed to add shift");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding shift: {ex.Message}");
            }
        }

        // PUT: api/shifts/5
        [HttpPut("{shiftId}")]
        [Authorize]
        public async Task<ActionResult<bool>> UpdateShift(int shiftId, [FromBody] ShiftModel shift)
        {
            try
            {
                if (shiftId != shift.ShiftID)
                    return BadRequest("Shift ID mismatch");

                if (!await _shiftsService.DoesShiftExist(shiftId))
                    return NotFound($"Shift with ID {shiftId} not found");

                var success = await _shiftsService.UpdateShift(shift);
                if (!success)
                    return BadRequest("Failed to update shift");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating shift: {ex.Message}");
            }
        }

        // DELETE: api/shifts/5
        [HttpDelete("{shiftId}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteShift(int shiftId)
        {
            try
            {
                if (!await _shiftsService.DoesShiftExist(shiftId))
                    return NotFound($"Shift with ID {shiftId} not found");

                var success = await _shiftsService.DeleteShift(shiftId);
                if (!success)
                    return BadRequest("Failed to delete shift");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting shift: {ex.Message}");
            }
        }
    }
}
