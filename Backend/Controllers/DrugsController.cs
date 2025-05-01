using Backend.DatabaseServices.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrugsController : ControllerBase
    {
        private readonly IDrugsDatabaseService _drugsService;

        public DrugsController(IDrugsDatabaseService drugsService)
        {
            _drugsService = drugsService;
        }

        // GET: api/drugs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DrugModel>>> GetDrugs()
        {
            var drugs = await _drugsService.GetDrugs();
            return Ok(drugs);
        }

        // POST: api/drugs
        [HttpPost]
        public async Task<IActionResult> AddDrug([FromBody] DrugModel drug)
        {
            var success = await _drugsService.AddDrug(drug);
            if (!success)
                return StatusCode(500, "Failed to add drug");

            return CreatedAtAction(nameof(GetDrugs), drug);
        }

        // PUT: api/drugs/{drugId}
        [HttpPut("{drugId}")]
        public async Task<IActionResult> UpdateDrug(int drugId, [FromBody] DrugModel drug)
        {
            if (drugId != drug.DrugID)
                return BadRequest("Drug ID mismatch");

            if (!await _drugsService.DoesDrugExist(drugId))
                return NotFound();

            var success = await _drugsService.UpdateDrug(drug);
            if (!success)
                return StatusCode(500, "Failed to update drug");

            return NoContent();
        }

        // DELETE: api/drugs/{drugId}
        [HttpDelete("{drugId}")]
        public async Task<IActionResult> DeleteDrug(int drugId)
        {
            if (!await _drugsService.DoesDrugExist(drugId))
                return NotFound();

            var success = await _drugsService.DeleteDrug(drugId);
            if (!success)
                return StatusCode(500, "Failed to delete drug");

            return NoContent();
        }
    }
}
