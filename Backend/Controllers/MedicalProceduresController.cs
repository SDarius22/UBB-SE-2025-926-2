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
    public class MedicalProceduresController : ControllerBase
    {
        private readonly IMedicalProceduresDatabaseService _medicalProceduresService;

        public MedicalProceduresController(IMedicalProceduresDatabaseService medicalProceduresService)
        {
            _medicalProceduresService = medicalProceduresService;
        }

        // GET: api/medicalprocedures/department/5
        [HttpGet("department/{departmentId}")]
        [Authorize]

        public async Task<ActionResult<IEnumerable<ProcedureModel>>> GetProceduresByDepartment(int departmentId)
        {
            try
            {
                var procedures = await _medicalProceduresService.GetProceduresByDepartmentId(departmentId);
                return Ok(procedures);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving procedures: {ex.Message}");
            }
        }
    }
}
