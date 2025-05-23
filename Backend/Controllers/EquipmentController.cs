﻿using Backend.DatabaseServices.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentDatabaseService _equipmentService;

        public EquipmentController(IEquipmentDatabaseService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        // GET: api/equipment
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<EquipmentModel>>> GetEquipments()
        {
            var equipments = await _equipmentService.GetEquipments();
            return Ok(equipments);
        }

        // POST: api/equipment
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddEquipment([FromBody] EquipmentModel equipment)
        {
            var success = await _equipmentService.AddEquipment(equipment);
            if (!success)
                return StatusCode(500, "Failed to add equipment");

            return CreatedAtAction(nameof(GetEquipments), equipment);
        }

        // PUT: api/equipment/{equipmentId}
        [HttpPut("{equipmentId}")]
        [Authorize]
        public async Task<IActionResult> UpdateEquipment(int equipmentId, [FromBody] EquipmentModel equipment)
        {
            if (equipmentId != equipment.EquipmentID)
                return BadRequest("Equipment ID mismatch");

            if (!await _equipmentService.DoesEquipmentExist(equipmentId))
                return NotFound();

            var success = await _equipmentService.UpdateEquipment(equipment);
            if (!success)
                return StatusCode(500, "Failed to update equipment");

            return NoContent();
        }

        // DELETE: api/equipment/{equipmentId}
        [HttpDelete("{equipmentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteEquipment(int equipmentId)
        {
            if (!await _equipmentService.DoesEquipmentExist(equipmentId))
                return NotFound();

            var success = await _equipmentService.DeleteEquipment(equipmentId);
            if (!success)
                return StatusCode(500, "Failed to delete equipment");

            return NoContent();
        }

        // GET: api/equipment/exists/{equipmentId}
        [HttpGet("exists/{equipmentId}")]
        [Authorize]
        public async Task<IActionResult> DoesEquipmentExist(int equipmentId)
        {
            var exists = await _equipmentService.DoesEquipmentExist(equipmentId);
            if (!exists)
            {
                return NotFound($"Equipment with ID {equipmentId} does not exist.");
            }
            return Ok(exists);  // It will return true if exists, false if not
        }
    }
}