using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Configs;
using Hospital.Exceptions;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using Microsoft.UI.Xaml.Controls;

namespace Hospital.DatabaseServices
{
    public class RatingDatabaseService : IRatingDatabaseService
    {

        private readonly ApplicationConfiguration _configuration;

        /// <summary>
        /// Adds a new review to the database.
        /// </summary>
        /// <param name="rating">The review to be added.</param>
        /// <returns>
        /// <c>true</c> if the review was successfully added; otherwise, <c>false</c>.
        /// </returns>
        public bool AddRating(RatingModel rating)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
                {
                    // string query = "INSERT INTO Reviews (ReviewID, MedicalRecordID, Text, NrStars) VALUES (@ReviewID, @MedicalRecordID, @Text, @NrStars)";
                    string query = "INSERT INTO Ratings (MedicalRecordId, NumberStars, Motivation) VALUES (@MedicalRecordId, @NumberStars, @Motivation)";
                    SqlCommand command = new SqlCommand(query, connection);

                    // command.Parameters.AddWithValue("@ReviewID", review.ReviewID);
                    command.Parameters.AddWithValue("@MedicalRecordID", rating.MedicalRecordId);
                    command.Parameters.AddWithValue("@NumberStars", rating.NumberStars);
                    command.Parameters.AddWithValue("@Motviation", rating.Motivation);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error in AddRating: {ex.Message}");
                return false;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid Operation in AddRating: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in AddRating: {ex.Message}");
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
        public RatingModel? FetchRating(int medicalRecordID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
                {
                    string query = "SELECT * FROM Reviews WHERE MedicalRecordID = @MedicalRecordID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MedicalRecordID", medicalRecordID);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        int reviewID = reader.GetInt32(reader.GetOrdinal("RatingId"));
                        int medicalRecordIDFromDb = reader.GetInt32(reader.GetOrdinal("MedicalRecordId"));
                        int nrStars = reader.GetInt32(reader.GetOrdinal("NumberStars"));
                        string motivation = reader.GetString(reader.GetOrdinal("Motivation"));

                        return new RatingModel(reviewID, medicalRecordIDFromDb, nrStars, motivation);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                return null;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid Operation: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
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
        public bool RemoveRating(int ratingId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
                {
                    string query = "DELETE FROM Reviews WHERE RatingId = @RatingID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RatingId", ratingId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error in RemoveRating: {ex.Message}");
                return false;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid Operation in RemoveRating: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in RemoveRating: {ex.Message}");
                return false;
            }
        }
    }
}
