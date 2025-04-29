using Hospital.Configs;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public class DepartmentsDatabaseService : IDepartmentsDatabaseService
    {
        private readonly ApplicationConfiguration _configuration;
        private readonly IDepartmentsDatabaseService _databaseService;

        public DepartmentsDatabaseService(IDepartmentsDatabaseService? databaseService = null)
        {
            _configuration = ApplicationConfiguration.GetInstance();
            _databaseService = databaseService ?? this;
        }

        public string GetConnectionString()
        {
            return _configuration.DatabaseConnection;
        }

        // This method will be used to get the departments from the database
        public virtual async Task<List<DepartmentModel>> GetDepartmentsFromDataBase()
        {
            const string selectDepartmentsQuery = "SELECT * FROM Departments";

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(_configuration.DatabaseConnection);
                await sqlConnection.OpenAsync().ConfigureAwait(false);

                //Prepare the command
                SqlCommand selectCommand = new SqlCommand(selectDepartmentsQuery, sqlConnection);
                SqlDataReader reader = await selectCommand.ExecuteReaderAsync().ConfigureAwait(false);


                //Prepare the list of departments
                List<DepartmentModel> departmentList = new List<DepartmentModel>();

                //Read the data from the database
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    int departmentId = reader.GetInt32(0);
                    string departmentName = reader.GetString(1);
                    DepartmentModel department = new DepartmentModel(departmentId, departmentName);
                    departmentList.Add(department);
                }
                return departmentList;
            }
            catch (SqlException sqlException)
            {
                throw new Exception($"SQL Exception: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new Exception($"Error loading departments: {exception.Message}");
            }
        }

        public bool AddDepartment(DepartmentModel department)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "INSERT INTO Departments (Name) VALUES (@Name)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", department.DepartmentName);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Updates an existing department in the database.
        /// </summary>
        /// <param name="department">The department to update.</param>
        /// <returns>True if the department was updated successfully, otherwise false.</returns>
        public bool UpdateDepartment(DepartmentModel department)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
                {
                    string query = "UPDATE Departments SET Name = @Name WHERE DepartmentId = @DepartmentID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", department.DepartmentName);
                    command.Parameters.AddWithValue("@DepartmentID", department.DepartmentId);

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
        /// Deletes a department from the database.
        /// </summary>
        /// <param name="departmentID">The ID of the department to delete.</param>
        /// <returns>True if the department was deleted successfully, otherwise false.</returns>
        public bool DeleteDepartment(int departmentID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "DELETE FROM Departments WHERE DepartmentId = @DepartmentID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DepartmentID", departmentID);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Checks if a department exists in the database.
        /// </summary>
        /// <param name="departmentID">The ID of the department to check.</param>
        /// <returns>True if the department exists, otherwise false.</returns>
        public bool DoesDepartmentExist(int departmentID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT COUNT(*) FROM Departments WHERE DepartmentId = @DepartmentID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DepartmentID", departmentID);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Retrieves all departments from the database.
        /// </summary>
        /// <returns>A list of departments.</returns>
        public List<DepartmentModel> GetDepartments()
        {
            List<DepartmentModel> departments = new List<DepartmentModel>();
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT * FROM Departments";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    departments.Add(new DepartmentModel(
                        reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                        reader.GetString(reader.GetOrdinal("Name"))));
                }
            }

            return departments;
        }
    }
}