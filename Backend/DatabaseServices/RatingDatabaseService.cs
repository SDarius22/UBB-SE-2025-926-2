using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Backend.Configs;
using Backend.DatabaseServices.Interfaces;
using Backend.DbContext;
using Backend.Exceptions;
using Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.DatabaseServices
{
    public class RatingDatabaseService : IRatingDatabaseService
    {

        private readonly AppDbContext _context;

        public RatingDatabaseService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new review to the database.
        /// </summary>
        /// <param name="rating">The review to be added.</param>
        /// <returns>
        /// <c>true</c> if the review was successfully added; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> AddRating(RatingModel rating)
        {
            try
            {
                var entity = new RatingModel
                {
                    MedicalRecordId = rating.MedicalRecordId,
                    NumberStars = rating.NumberStars,
                    Motivation = rating.Motivation
                };

                await _context.Ratings.AddAsync(entity);
                await _context.SaveChangesAsync();

                // Update the model with generated ID if needed
                rating.RatingId = entity.RatingId;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding rating: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Fetches a review from the database based on the given medical record ID.
        /// </summary>
        /// <param name="medicalRecordID">The medical record ID used to search for the review.</param>
        /// <returns>
        /// A <see cref="RatingModel"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<RatingModel?> FetchRating(int medicalRecordID)
        {
            try
            {
                return await _context.Ratings
                    .Where(rating => rating.MedicalRecordId == medicalRecordID)
                    .Select(r => new RatingModel
                    {
                        RatingId = r.RatingId,
                        MedicalRecordId = r.MedicalRecordId,
                        NumberStars = r.NumberStars,
                        Motivation = r.Motivation
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching rating: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Removes a review from the database based on the review ID.
        /// </summary>
        /// <param name="ratingId">The ID of the review to be removed.</param>
        /// <returns>
        /// <c>true</c> if the review was successfully removed; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> RemoveRating(int ratingId)
        {
            try
            {
                var rating = await _context.Ratings.FindAsync(ratingId);
                if (rating == null) return false;

                _context.Ratings.Remove(rating);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting rating: {ex.Message}");
                return false;
            }
        }
    }
}