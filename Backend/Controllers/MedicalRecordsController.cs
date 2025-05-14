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
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordsDatabaseService _medicalRecordsService;

        public MedicalRecordsController(IMedicalRecordsDatabaseService medicalRecordsService)
        {
            _medicalRecordsService = medicalRecordsService;
        }

        // POST: api/medicalrecords
        [HttpPost]
        [Authorize]

        public async Task<ActionResult<int>> AddMedicalRecord([FromBody] MedicalRecordModel medicalRecord)
        {
            try
            {
                var medicalRecordId = await _medicalRecordsService.AddMedicalRecord(medicalRecord);
                return CreatedAtAction(nameof(GetMedicalRecordById), new { medicalRecordId }, medicalRecordId);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating medical record: {ex.Message}");
            }
        }

        // GET: api/medicalrecords/patient/5
        [HttpGet("patient/{patientId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MedicalRecordJointModel>>> GetMedicalRecordsForPatient(int patientId)
        {
            try
            {
                var records = await _medicalRecordsService.GetMedicalRecordsForPatient(patientId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving medical records: {ex.Message}");
            }
        }

        // GET: api/medicalrecords/5
        [HttpGet("{medicalRecordId}")]
        [Authorize]
        public async Task<ActionResult<MedicalRecordJointModel>> GetMedicalRecordById(int medicalRecordId)
        {
            try
            {
                var record = await _medicalRecordsService.GetMedicalRecordById(medicalRecordId);
                return Ok(record);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving medical record: {ex.Message}");
            }
        }

        // GET: api/medicalrecords/doctor/5
        [HttpGet("doctor/{doctorId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MedicalRecordJointModel>>> GetMedicalRecordsForDoctor(int doctorId)
        {
            try
            {
                var records = await _medicalRecordsService.GetMedicalRecordsForDoctor(doctorId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving medical records: {ex.Message}");
            }
        }
    }
}
