using Hospital.Configs;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using DrugModel = Hospital.Models.DrugModel;

namespace Hospital.DatabaseServices
{
    public class DrugsDatabaseService : IDrugsDatabaseService
    {
        private readonly ApplicationConfiguration _configuration;

        public DrugsDatabaseService()
        {
            _configuration = ApplicationConfiguration.GetInstance();
        }


        /// <summary>
        /// Adds a new drug to the database.
        /// </summary>
        /// <param name="drug">The drug to add.</param>
        /// <returns>True if the drug was added successfully; otherwise, false.</returns>
        public bool AddDrug(DrugModel drug)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                // string query = "INSERT INTO Drugs (DrugID, Name, Administration, Specification, Supply) VALUES (@DrugID, @Name, @Administration, @Specification, @Supply)";
                string query = "INSERT INTO Drugs (Name, Administration, Specification, Supply) VALUES (@Name, @Administration, @Specification, @Supply)";
                SqlCommand command = new SqlCommand(query, connection);

                // command.Parameters.AddWithValue("@DrugID", drug.DrugID);
                command.Parameters.AddWithValue("@Name", drug.Name);
                command.Parameters.AddWithValue("@Administration", drug.Administration);
                command.Parameters.AddWithValue("@Specification", drug.Specification);
                command.Parameters.AddWithValue("@Supply", drug.Supply);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Updates an existing drug in the database.
        /// </summary>
        /// <param name="drug">The drug to update.</param>
        /// <returns>True if the drug was updated successfully; otherwise, false.</returns>
        public bool UpdateDrug(DrugModel drug)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
                {
                    // string query = "UPDATE Drugs SET Name = @Name, Administration = @Administration, Specification = @Specification, Supply = @Supply WHERE DrugID = @DrugID";
                    string query = "UPDATE Drugs SET Name = @Name, Administration = @Administration, Specification = @Specification, Supply = @Supply WHERE DrugID = @DrugID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", drug.Name);
                    command.Parameters.AddWithValue("@Administration", drug.Administration);
                    command.Parameters.AddWithValue("@Specification", drug.Specification);
                    command.Parameters.AddWithValue("@Supply", drug.Supply);
                    command.Parameters.AddWithValue("@DrugID", drug.DrugID);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                return false;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid Operation: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes a drug from the database.
        /// </summary>
        /// <param name="drugID">The ID of the drug to delete.</param>
        /// <returns>True if the drug was deleted successfully; otherwise, false.</returns>
        public bool DeleteDrug(int drugID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "DELETE FROM Drugs WHERE DrugID = @DrugID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DrugID", drugID);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Checks if a drug exists in the database.
        /// </summary>
        /// <param name="drugID">The ID of the drug to check.</param>
        /// <returns>True if the drug exists; otherwise, false.</returns>
        public bool DoesDrugExist(int drugID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT COUNT(*) FROM Drugs WHERE DrugID = @DrugID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DrugID", drugID);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Retrieves all drugs from the database.
        /// </summary>
        /// <returns>A list of drugs.</returns>
        public List<DrugModel> GetDrugs()
        {
            List<DrugModel> drugs = new List<DrugModel>();
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT * FROM Drugs";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DrugModel drug = new DrugModel
                    {
                        DrugID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Administration = reader.GetString(2),
                        Supply = reader.GetInt32(3),
                        Specification = reader.GetString(4),
                    };
                    drugs.Add(drug);
                }
            }

            return drugs;
        }
    }
}
