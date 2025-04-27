namespace Hospital.DatabaseServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Hospital.Configs;
    using Hospital.Models;
    using Microsoft.Data.SqlClient;
    using Windows.System;

    public class DoctorsDatabaseService : IDoctorsDatabaseService
    {
        private readonly ApplicationConfiguration _configuration;

        public DoctorsDatabaseService()
        {
            this._configuration = ApplicationConfiguration.GetInstance();
        }

        // This method will be used to get the doctors from the database
        public async Task<List<DoctorJointModel>> GetDoctorsByDepartment(int departmentId)
        {
            const string selectDoctorsByDepartmentQuery = @"SELECT
                d.DoctorId,
                d.UserId,
                u.Username,
                d.DepartmentId,
                d.DoctorRating,
                d.LicenseNumber
                FROM Doctors d
                INNER JOIN Users u
                ON d.UserId = u.UserId
                WHERE DepartmentId = @departmentId";

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(_configuration.DatabaseConnection);
                await sqlConnection.OpenAsync().ConfigureAwait(false);

                // Prepare the command
                SqlCommand selectDoctorsCommand = new SqlCommand(selectDoctorsByDepartmentQuery, sqlConnection);

                // Insert parameters
                selectDoctorsCommand.Parameters.AddWithValue("@departmentId", departmentId);

                SqlDataReader reader = await selectDoctorsCommand.ExecuteReaderAsync().ConfigureAwait(false);


                // Prepare the list of doctors
                List<DoctorJointModel> doctorsList = new List<DoctorJointModel>();

                // Read the data from the database
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    int doctorId = reader.GetInt32(0);
                    int userId = reader.GetInt32(1);
                    string doctorName = reader.GetString(2);
                    int depId = reader.GetInt32(3);
                    double rating = reader.GetDouble(4);
                    string licenseNumber = reader.GetString(5);
                    DoctorJointModel doctor = new DoctorJointModel(doctorId, userId, doctorName, departmentId, rating, licenseNumber);
                    doctorsList.Add(doctor);
                }
                return doctorsList;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Exception: {sqlException.Message}");
                return new List<DoctorJointModel>();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Exception: {exception.Message}");
                return new List<DoctorJointModel>();
            }
        }
    }
}
