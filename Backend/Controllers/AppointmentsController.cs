using Backend.DatabaseServices;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentsDatabaseService _appointmentsService;

        public AppointmentsController(IAppointmentsDatabaseService appointmentsService)
        {
            _appointmentsService = appointmentsService;
        }

        // GET: api/appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentJointModel>>> GetAllAppointments()
        {
            var appointments = await _appointmentsService.GetAllAppointments();
            return Ok(appointments);
        }

        // GET: api/appointments/{appointmentId}
        [HttpGet("{appointmentId}")]
        public async Task<ActionResult<AppointmentJointModel>> GetAppointment(int appointmentId)
        {
            var appointment = await _appointmentsService.GetAppointment(appointmentId);
            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        // GET: api/appointments/doctor/{doctorId}/date/{date}
        [HttpGet("doctor/{doctorId}/date/{date}")]
        public async Task<ActionResult<IEnumerable<AppointmentJointModel>>> GetAppointmentsByDoctorAndDate(int doctorId, DateTime date)
        {
            var appointments = await _appointmentsService.GetAppointmentsByDoctorAndDate(doctorId, date);
            return Ok(appointments);
        }

        // GET: api/appointments/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<AppointmentJointModel>>> GetAppointmentsForPatient(int patientId)
        {
            var appointments = await _appointmentsService.GetAppointmentsForPatient(patientId);
            return Ok(appointments);
        }

        // GET: api/appointments/doctor/{doctorId}
        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<AppointmentJointModel>>> GetAppointmentsForDoctor(int doctorId)
        {
            var appointments = await _appointmentsService.GetAppointmentsForDoctor(doctorId);
            return Ok(appointments);
        }

        // POST: api/appointments
        [HttpPost]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentModel appointment)
        {
            var success = await _appointmentsService.AddAppointmentToDataBase(appointment);
            if (!success)
                return StatusCode(500, "Failed to add appointment");

            return CreatedAtAction(nameof(GetAppointment), new { appointmentId = appointment.AppointmentId }, appointment);
        }

        // DELETE: api/appointments/{appointmentId}
        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> RemoveAppointment(int appointmentId)
        {
            var success = await _appointmentsService.RemoveAppointmentFromDataBase(appointmentId);
            if (!success)
                return StatusCode(500, "Failed to delete appointment");

            return NoContent();
        }
    }
}
