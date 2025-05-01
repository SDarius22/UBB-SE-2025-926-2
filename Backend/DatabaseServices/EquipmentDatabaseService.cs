using System;
using Backend.DatabaseServices.Interfaces;
using System.Collections.Generic;
using Backend.DbContext;
using System.Threading.Tasks;
//using Backend.Configs;
using Backend.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using EquipmentModel = Backend.Models.EquipmentModel;
using Backend.Models;
using System.Linq;
using System.Diagnostics;

namespace Backend.DatabaseServices
{
    public class EquipmentDatabaseService : IEquipmentDatabaseService
    {

        private readonly AppDbContext _context;

        public EquipmentDatabaseService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new equipment to the database.
        /// </summary>
        /// <param name="equipment">The equipment to add.</param>
        /// <returns>True if the equipment was added successfully, otherwise false.</returns>
        public async Task<bool> AddEquipment(EquipmentModel equipment)
        {
            try
            {
                var entity = new EquipmentModel
                {
                    Name = equipment.Name,
                    Type = equipment.Type,
                    Specification = equipment.Specification,
                    Stock = equipment.Stock
                };

                await _context.Equipments.AddAsync(entity);
                await _context.SaveChangesAsync();

                // Update the model with generated ID if needed
                equipment.EquipmentID = entity.EquipmentID;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding eqpuipment: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Updates an existing equipment in the database.
        /// </summary>
        /// <param name="equipment">The equipment to update.</param>
        /// <returns>True if the equipment was updated successfully, otherwise false.</returns>
        public async Task<bool> UpdateEquipment(EquipmentModel equipment)
        {
            try
            {
                var existingEquipment = await _context.Equipments.FindAsync(equipment.EquipmentID);

                if (existingEquipment == null) return false;

                existingEquipment.Name = equipment.Name;
                existingEquipment.Type = equipment.Type;
                existingEquipment.Specification = equipment.Specification;
                existingEquipment.Stock = equipment.Stock;


                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating equipment: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes an equipment from the database.
        /// </summary>
        /// <param name="equipmentID">The ID of the equipment to delete.</param>
        /// <returns>True if the equipment was deleted successfully, otherwise false.</returns>
        public async Task<bool> DeleteEquipment(int equipmentID)
        {
            try
            {
                var equipment = await _context.Equipments.FindAsync(equipmentID);
                if (equipment == null) return false;

                _context.Equipments.Remove(equipment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting equipment: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if an equipment exists in the database.
        /// </summary>
        /// <param name="equipmentID">The ID of the equipment to check.</param>
        /// <returns>True if the equipment exists, otherwise false.</returns>
        public async Task<bool> DoesEquipmentExist(int equipmentID)
        {
            return await _context.Equipments
                .AnyAsync(equipment => equipment.EquipmentID == equipmentID);
        }

        /// <summary>
        /// Retrieves all equipment from the database.
        /// </summary>
        /// <returns>A list of equipment.</returns>

        public async Task<List<EquipmentModel>> GetEquipments()
        {
            try
            {
                Debug.WriteLine("Getting all equipments from the database.");
                return await _context.Equipments
                    .Select(equipment => new EquipmentModel
                    {
                        EquipmentID = equipment.EquipmentID,
                        Name = equipment.Name,
                        Type = equipment.Type,
                        Specification = equipment.Specification,
                        Stock = equipment.Stock,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting equipments: {ex.Message}");
                return null;
            }
        }
    }
}