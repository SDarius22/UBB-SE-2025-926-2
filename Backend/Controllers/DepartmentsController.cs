using Backend.DatabaseServices;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentsDatabaseService _departmentsService;

        public DepartmentsController(IDepartmentsDatabaseService departmentsService)
        {
            _departmentsService = departmentsService;
        }

        // GET: api/departments
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DepartmentModel>>> GetDepartments()
        {
            var departments = await _departmentsService.GetDepartments();
            return Ok(departments);
        }

        // POST: api/departments
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<DepartmentModel>> AddDepartment([FromBody] DepartmentModel department)
        {
            var success = await _departmentsService.AddDepartment(department);
            if (!success)
                return StatusCode(500, "Failed to add department");

            return CreatedAtAction(nameof(GetDepartments), department);
        }

        // PUT: api/departments/{departmentId}
        [HttpPut("{departmentId}")]
        [Authorize]
        public async Task<IActionResult> UpdateDepartment(int departmentId, [FromBody] DepartmentModel department)
        {
            if (departmentId != department.DepartmentID)
                return BadRequest("Department ID mismatch");

            if (!await _departmentsService.DoesDepartmentExist(departmentId))
                return NotFound();

            var success = await _departmentsService.UpdateDepartment(department);
            if (!success)
                return StatusCode(500, "Failed to update department");

            return NoContent();
        }

        // DELETE: api/departments/{departmentId}
        [HttpDelete("{departmentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteDepartment(int departmentId)
        {
            if (!await _departmentsService.DoesDepartmentExist(departmentId))
                return NotFound();

            var success = await _departmentsService.DeleteDepartment(departmentId);
            if (!success)
                return StatusCode(500, "Failed to delete department");

            return NoContent();
        }

        [HttpGet("exists/{departmentId}")]
        [Authorize]
        public async Task<ActionResult<bool>> DoesDepartmentExist(int departmentId)
        {
            var exists = await _departmentsService.DoesDepartmentExist(departmentId);
            if (!exists)
                return NotFound();

            return Ok(exists); // returns true if department exists
        }
    }
}