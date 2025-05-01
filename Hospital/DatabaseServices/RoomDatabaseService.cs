using Hospital.Configs;
using Hospital.DatabaseServices.Interfaces;
using Hospital.DbContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomModel = Hospital.Models.RoomModel;

namespace Hospital.DatabaseServices
{
    public class RoomDatabaseService : IRoomDatabaseService
    {
        private readonly AppDbContext _context;

        public RoomDatabaseService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new room to the database.
        /// </summary>
        /// <param name="room">The room object to be added.</param>
        /// <returns><c>true</c> if the room was successfully added; otherwise, <c>false</c>.</returns>
        public async Task<bool> AddRoom(RoomModel room)
        {
            try
            {
                var entity = new RoomModel(room.RoomID, room.Capacity, room.DepartmentID, room.EquipmentID);

                await _context.Rooms.AddAsync(entity);
                await _context.SaveChangesAsync();

                // Update the model with generated ID if needed
                room.RoomID = entity.RoomID;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding room: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Updates an existing room in the database.
        /// </summary>
        /// <param name="room">The room object containing updated values.</param>
        /// <returns><c>true</c> if the update was successful; otherwise, <c>false</c>.</returns>
        public async Task<bool> UpdateRoom(RoomModel room)
        {
            try
            {
                var existingRoom = await _context.Rooms.FindAsync(room.RoomID);
                if (existingRoom == null) return false;

                existingRoom.Capacity = room.Capacity;
                existingRoom.DepartmentID = room.DepartmentID;
                existingRoom.EquipmentID = room.EquipmentID;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating room: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes a room from the database based on its ID.
        /// </summary>
        /// <param name="roomID">The ID of the room to delete.</param>
        /// <returns><c>true</c> if the room was deleted; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteRoom(int roomId)
        {
            try
            {
                var room = await _context.Rooms.FindAsync(roomId);
                if (room == null) return false;

                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting room: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Checks whether a room with the specified ID exists in the database.
        /// </summary>
        /// <param name="roomID">The room ID to check for existence.</param>
        /// <returns><c>true</c> if the room exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> DoesRoomExist(int roomId)
        {
            return await _context.Rooms
                .AnyAsync(r => r.RoomID == roomId);
        }

        /// <summary>
        /// Checks whether a piece of equipment with the given ID exists.
        /// </summary>
        /// <param name="equipmentID">The equipment ID to check.</param>
        /// <returns><c>true</c> if the equipment exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> DoesEquipmentExist(int equipmentId)
        {
            return await _context.Equipments
                .AnyAsync(e => e.EquipmentID == equipmentId);
        }

        /// <summary>
        /// Checks whether a department with the given ID exists.
        /// </summary>
        /// <param name="departmentID">The department ID to check.</param>
        /// <returns><c>true</c> if the department exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> DoesDepartmentExist(int departmentId)
        {
            return await _context.Departments
                .AnyAsync(d => d.DepartmentID == departmentId);
        }

        /// <summary>
        /// Retrieves a list of all rooms from the database.
        /// </summary>
        /// <returns>A list of <see cref="Room"/> objects, or <c>null</c> if an error occurred.</returns>
        public async Task<List<RoomModel>?> GetRooms()
        {
            try
            {
                return await _context.Rooms
                    .Select(r => new RoomModel
                    {
                        RoomID = r.RoomID,
                        Capacity = r.Capacity,
                        DepartmentID = r.DepartmentID,
                        EquipmentID = r.EquipmentID,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting rooms: {ex.Message}");
                return null;
            }
        }
    }
}