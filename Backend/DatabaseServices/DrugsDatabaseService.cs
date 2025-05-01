//using Backend.Configs;
using Backend.DatabaseServices.Interfaces;
using Backend.DbContext;
using Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrugModel = Backend.Models.DrugModel;

namespace Backend.DatabaseServices
{
    public class DrugsDatabaseService : IDrugsDatabaseService
    {
        private readonly AppDbContext _context;

        public DrugsDatabaseService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new drug to the database.
        /// </summary>
        /// <param name="drug">The drug to add.</param>
        /// <returns>True if the drug was added successfully; otherwise, false.</returns>
        public async Task<bool> AddDrug(DrugModel drug)
        {
            try
            {
                var entity = new DrugModel(drug.DrugID, drug.Name, drug.Administration, drug.Specification, drug.Supply);

                await _context.Drugs.AddAsync(entity);
                await _context.SaveChangesAsync();

                // Update the model with generated ID if needed
                drug.DrugID = entity.DrugID;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding drug: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Updates an existing drug in the database.
        /// </summary>
        /// <param name="drug">The drug to update.</param>
        /// <returns>True if the drug was updated successfully; otherwise, false.</returns>
        public async Task<bool> UpdateDrug(DrugModel drug)
        {

            try
            {
                var existingDrug = await _context.Drugs.FindAsync(drug.DrugID);

                if (existingDrug == null) return false;

                existingDrug.Name = drug.Name;
                existingDrug.Administration = drug.Administration;
                existingDrug.Specification = drug.Specification;
                existingDrug.Supply = drug.Supply;


                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating drug: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes a drug from the database.
        /// </summary>
        /// <param name="drugID">The ID of the drug to delete.</param>
        /// <returns>True if the drug was deleted successfully; otherwise, false.</returns>
        public async Task<bool> DeleteDrug(int drugID)
        {
            try
            {
                var drug = await _context.Drugs.FindAsync(drugID);
                if (drug == null) return false;

                _context.Drugs.Remove(drug);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting drug: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a drug exists in the database.
        /// </summary>
        /// <param name="drugID">The ID of the drug to check.</param>
        /// <returns>True if the drug exists; otherwise, false.</returns>
        public async Task<bool> DoesDrugExist(int drugID)
        {

            return await _context.Drugs
                .AnyAsync(drug => drug.DrugID == drugID);
        }

        /// <summary>
        /// Retrieves all drugs from the database.
        /// </summary>
        /// <returns>A list of drugs.</returns>
        public async Task<List<DrugModel>> GetDrugs()
        {
            try
            {
                return await _context.Drugs
                    .Select(drug => new DrugModel
                    {
                        DrugID = drug.DrugID,
                        Name = drug.Name,
                        Administration = drug.Administration,
                        Specification = drug.Specification,
                        Supply = drug.Supply,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting drugs: {ex.Message}");
                return null;
            }
        }
    }
}
