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
    }
}
