﻿using Backend.DatabaseServices;
using Backend.DatabaseServices.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomDatabaseService _roomService;

        public RoomController(IRoomDatabaseService roomService)
        {
            _roomService = roomService;
        }

        // GET: api/room
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RoomModel>>> GetRooms()
        {
            try
            {
                var rooms = await _roomService.GetRooms();
                if (rooms == null)
                    return NotFound("No rooms found");

                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving rooms: {ex.Message}");
            }
        }

        // POST: api/room
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<bool>> AddRoom([FromBody] RoomModel room)
        {
            try
            {
                if (!await _roomService.DoesDepartmentExist(room.DepartmentID))
                    return BadRequest("Department does not exist");

                if (!await _roomService.DoesEquipmentExist(room.EquipmentID))
                    return BadRequest("Equipment does not exist");

                var success = await _roomService.AddRoom(room);
                if (!success)
                    return BadRequest("Failed to add room");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding room: {ex.Message}");
            }
        }

        // PUT: api/room/5
        [HttpPut("{roomId}")]
        [Authorize]
        public async Task<ActionResult<bool>> UpdateRoom(int roomId, [FromBody] RoomModel room)
        {
            try
            {
                if (roomId != room.RoomID)
                    return BadRequest("Room ID mismatch");

                if (!await _roomService.DoesRoomExist(roomId))
                    return NotFound($"Room with ID {roomId} not found");

                if (!await _roomService.DoesDepartmentExist(room.DepartmentID))
                    return BadRequest("Department does not exist");

                if (!await _roomService.DoesEquipmentExist(room.EquipmentID))
                    return BadRequest("Equipment does not exist");

                var success = await _roomService.UpdateRoom(room);
                if (!success)
                    return BadRequest("Failed to update room");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating room: {ex.Message}");
            }
        }

        // DELETE: api/room/5
        [HttpDelete("{roomId}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteRoom(int roomId)
        {
            try
            {
                if (!await _roomService.DoesRoomExist(roomId))
                    return NotFound($"Room with ID {roomId} not found");

                var success = await _roomService.DeleteRoom(roomId);
                if (!success)
                    return BadRequest("Failed to delete room");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting room: {ex.Message}");
            }
        }

        // Helper Method 1: Check if Room Exists
        [HttpGet("room-exists/{roomID}")]
        [Authorize]
        public async Task<bool> DoesRoomExist(int roomID)
        {
            return await _roomService.DoesRoomExist(roomID);
        }

        // Helper Method 2: Check if Equipment Exists
        [HttpGet("exists/{equipmentID}")]
        [Authorize]
        public async Task<bool> DoesEquipmentExist(int equipmentID)
        {
            return await _roomService.DoesEquipmentExist(equipmentID);
        }

        // Helper Method 3: Check if Department Exists
        [HttpGet("department-exists/{departmentID}")]
        [Authorize]
        public async Task<bool> DoesDepartmentExist(int departmentID)
        {
            return await _roomService.DoesDepartmentExist(departmentID);
        }
    }
}