using Backend.DatabaseServices;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorsDatabaseService _doctorsService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IDoctorsDatabaseService doctorsService, ILogger<DoctorsController> logger)
        {
            _doctorsService = doctorsService;
            _logger = logger;
        }

        // GET: api/doctors
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DoctorJointModel>>> GetDoctors()
        {
            var doctors = await _doctorsService.GetDoctors();

            return Ok(doctors);
        }

        // GET: api/doctors/department/5
        [HttpGet("department/{departmentId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DoctorJointModel>>> GetDoctorsByDepartment(int departmentId)
        {
            var doctors = await _doctorsService.GetDoctorsByDepartment(departmentId);
            return Ok(doctors);
        }

        // POST: api/doctors
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<DoctorJointModel>> AddDoctor([FromBody] DoctorJointModel doctor)
        {
            Debug.WriteLine($"Adding doctor with UserId: {doctor.UserId}, DepartmentId: {doctor.DepartmentId}, LicenseNumber: {doctor.LicenseNumber}");
            Console.WriteLine($"Adding doctor with UserId: {doctor.UserId}, DepartmentId: {doctor.DepartmentId}, LicenseNumber: {doctor.LicenseNumber}");
            _logger.LogInformation($"Adding doctor with UserId: {doctor.UserId}, DepartmentId: {doctor.DepartmentId}, LicenseNumber: {doctor.LicenseNumber}");
            if (!await _doctorsService.DoesUserExist(doctor.UserId))
                return BadRequest("User does not exist");

            if (!await _doctorsService.DoesDepartmentExist(doctor.DepartmentId))
                return BadRequest("Department does not exist");

            if (await _doctorsService.IsUserAlreadyDoctor(doctor.UserId))
                return BadRequest("User is already a doctor");

            var success = await _doctorsService.AddDoctor(doctor);
            if (!success)
                return StatusCode(500, "Failed to add doctor");

            return CreatedAtAction(nameof(GetDoctors), doctor);
        }

        // PUT: api/doctors/5
        [HttpPut("{doctorId}")]
        [Authorize]
        public async Task<IActionResult> UpdateDoctor(int doctorId, [FromBody] DoctorJointModel doctor)
        {
            if (doctorId != doctor.DoctorId)
                return BadRequest("Doctor ID mismatch");

            if (!await _doctorsService.DoesDoctorExist(doctorId))
                return NotFound();

            if (!await _doctorsService.UserExistsInDoctors(doctor.UserId, doctorId))
                return BadRequest("User is not associated with this doctor record");

            var success = await _doctorsService.UpdateDoctor(doctor);
            if (!success)
                return StatusCode(500, "Failed to update doctor");

            return NoContent();
        }

        // DELETE: api/doctors/5
        [HttpDelete("{doctorId}")]
        [Authorize]
        public async Task<IActionResult> DeleteDoctor(int doctorId)
        {
            if (!await _doctorsService.DoesDoctorExist(doctorId))
                return NotFound();

            var success = await _doctorsService.DeleteDoctor(doctorId);
            if (!success)
                return StatusCode(500, "Failed to delete doctor");

            return NoContent();
        }

        // GET: api/doctors/check-doctor/5
        [HttpGet("check-doctor/{userId}")]
        [Authorize]
        public async Task<ActionResult<bool>> IsUserDoctor(int userId)
        {
            var isDoctor = await _doctorsService.IsUserDoctor(userId);
            return Ok(isDoctor);
        }

        // CHECK: api/doctors/is-user-already-doctor/5
        [HttpGet("is-user-already-doctor/{userId}")]
        [Authorize]
        public async Task<ActionResult<bool>> IsUserAlreadyDoctor(int userId)
        {
            var isAlreadyDoctor = await _doctorsService.IsUserAlreadyDoctor(userId);
            return Ok(isAlreadyDoctor);
        }

        // GET: api/doctors/does-user-exist/5
        [HttpGet("does-user-exist/{userId}")]
        [Authorize]
        public async Task<ActionResult<bool>> DoesUserExist(int userId)
        {
            var userExists = await _doctorsService.DoesUserExist(userId);
            return Ok(userExists);
        }

        // GET: api/doctors/does-department-exist/5
        [HttpGet("does-department-exist/{departmentId}")]
        [Authorize]
        public async Task<ActionResult<bool>> DoesDepartmentExist(int departmentId)
        {
            var departmentExists = await _doctorsService.DoesDepartmentExist(departmentId);
            return Ok(departmentExists);
        }

        // GET: api/doctors/user-exists-in-doctors/5
        [HttpGet("user-exists-in-doctors/{userId}/{doctorId}")]
        [Authorize]
        public async Task<ActionResult<bool>> UserExistsInDoctors(int userId, int doctorId)
        {
            var userExists = await _doctorsService.UserExistsInDoctors(userId, doctorId);
            return Ok(userExists);
        }

        // GET: api/doctors/doctor-exists/5
        [HttpGet("doctor-exists/{doctorId}")]
        [Authorize]
        public async Task<ActionResult<bool>> DoesDoctorExist(int doctorId)
        {
            var doctorExists = await _doctorsService.DoesDoctorExist(doctorId);
            return Ok(doctorExists);
        }
    }
}