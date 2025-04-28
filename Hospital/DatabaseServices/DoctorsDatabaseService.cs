namespace Hospital.DatabaseServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Hospital.Configs;
    using Hospital.Models;
    using Microsoft.Data.SqlClient;
    using Windows.System;
    using DoctorJointModel = Hospital.Models.DoctorJointModel;

    public class DoctorsDatabaseService : IDoctorsDatabaseService
    {

        private const double Type0Rate = 200d;
        private const double Type1Rate = Type0Rate * 1.2d;
        private const double Type2Rate = Type1Rate * 1.5d;

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

        /// <summary>
        /// Adds a new doctor to the database.
        /// </summary>
        /// <param name="doctor">The doctor to add.</param>
        /// <returns>True if the doctor was added successfully; otherwise, false.</returns>
        public bool AddDoctor(DoctorJointModel doctor)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "INSERT INTO Doctors (UserID, DepartmentID, Rating, LicenseNumber) VALUES (@UserID, @DepartmentID, @Rating, @LicenseNumber)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", doctor.UserId);
                command.Parameters.AddWithValue("@DepartmentID", doctor.DepartmentId);
                command.Parameters.AddWithValue("@Rating", doctor.DoctorRating);
                command.Parameters.AddWithValue("@LicenseNumber", doctor.LicenseNumber);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Updates an existing doctor's details in the database.
        /// </summary>
        /// <param name="doctor">The doctor with updated details.</param>
        /// <returns>True if the doctor was updated successfully; otherwise, false.</returns>
        public bool UpdateDoctor(DoctorJointModel doctor)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
                {
                    string query = "UPDATE Doctors SET UserID = @UserID, DepartmentID = @DepartmentID, LicenseNumber = @LicenseNumber WHERE DoctorID = @DoctorID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserID", doctor.UserId);
                    command.Parameters.AddWithValue("@DepartmentID", doctor.DepartmentId);
                    command.Parameters.AddWithValue("@LicenseNumber", doctor.LicenseNumber);
                    command.Parameters.AddWithValue("@DoctorID", doctor.DoctorId);

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
        /// Deletes a doctor from the database based on the provided doctor ID.
        /// </summary>
        /// <param name="doctorID">The ID of the doctor to delete.</param>
        /// <returns>True if the doctor was deleted successfully; otherwise, false.</returns>
        public bool DeleteDoctor(int doctorID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection)) // Added 'this.'
            {
                string query = "DELETE FROM Doctors WHERE DoctorID = @DoctorID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DoctorID", doctorID);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Checks if a doctor exists in the database based on the provided doctor ID.
        /// </summary>
        /// <param name="doctorID">The ID of the doctor to check.</param>
        /// <returns>True if the doctor exists; otherwise, false.</returns>
        public bool DoesDoctorExist(int doctorID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT COUNT(*) FROM Doctors WHERE DoctorID = @DoctorID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DoctorID", doctorID);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Checks if a user is already a doctor in the database.
        /// </summary>
        /// <param name="userID">The ID of the user to check.</param>
        /// <returns>True if the user is already a doctor; otherwise, false.</returns>
        public bool IsUserAlreadyDoctor(int userID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT COUNT(*) FROM Doctors WHERE UserID = @UserID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userID);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Checks if a user exists in the database.
        /// </summary>
        /// <param name="userID">The ID of the user to check.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        public bool DoesUserExist(int userID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE UserID = @UserID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userID);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Checks if a user has the role of a doctor.
        /// </summary>
        /// <param name="userID">The ID of the user to check.</param>
        /// <returns>True if the user is a doctor; otherwise, false.</returns>
        public bool IsUserDoctor(int userID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT Role FROM Users WHERE UserID = @UserID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userID);

                connection.Open();
                string role = (string)command.ExecuteScalar();
                return role == "Doctor";
            }
        }

        /// <summary>
        /// Checks if a department exists in the database.
        /// </summary>
        /// <param name="departmentID">The ID of the department to check.</param>
        /// <returns>True if the department exists; otherwise, false.</returns>
        public bool DoesDepartmentExist(int departmentID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT COUNT(*) FROM Departments WHERE DepartmentID = @DepartmentID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DepartmentID", departmentID);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Checks if a user exists in the doctors table but with a different doctor ID.
        /// </summary>
        /// <param name="userID">The ID of the user to check.</param>
        /// <param name="doctorID">The ID of the doctor to exclude from the check.</param>
        /// <returns>True if the user exists in the doctors table with a different doctor ID; otherwise, false.</returns>
        public bool UserExistsInDoctors(int userID, int doctorID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT COUNT(*) FROM Doctors WHERE UserID = @UserID AND DoctorID != @DoctorID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@DoctorID", doctorID);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Retrieves the shifts for the current month for a specific doctor.
        /// </summary>
        /// <param name="doctorID">The ID of the doctor whose shifts are to be retrieved.</param>
        /// <returns>A list of shifts for the current month.</returns>
        public List<ShiftModel> GetShiftsForCurrentMonth(int doctorID)
        {
            List<ShiftModel> shifts = new List<ShiftModel>();

            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = @"
                    SELECT s.ShiftID, s.Date, s.StartTime, s.EndTime
                    FROM GetCurrentMonthShiftsForDoctor(@DoctorID) s";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DoctorID", doctorID);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    shifts.Add(new ShiftModel(
                        reader.GetInt32(0),
                        reader.GetDateTime(1),
                        reader.GetTimeSpan(2),
                        reader.GetTimeSpan(3)));
                }
            }

            return shifts;
        }

        /// <summary>
        /// Computes the salary of a doctor for the current month based on their shifts.
        /// </summary>
        /// <param name="doctorID">The ID of the doctor whose salary is to be computed.</param>
        /// <returns>The total salary of the doctor for the current month.</returns>
        public double ComputeDoctorSalary(int doctorID)
        {
            List<ShiftModel> shifts = this.GetShiftsForCurrentMonth(doctorID);
            double totalSalary = 0;

            foreach (var shift in shifts)
            {
                double shiftRate = 0;

                if (shift.StartTime == new TimeSpan(8, 0, 0) && shift.EndTime == new TimeSpan(20, 0, 0))
                {
                    shiftRate = Type0Rate * 12;
                }
                else if (shift.StartTime == new TimeSpan(20, 0, 0) && shift.EndTime == new TimeSpan(8, 0, 0))
                {
                    shiftRate = Type1Rate * 12;
                }
                else if (shift.StartTime == new TimeSpan(8, 0, 0) && shift.EndTime == new TimeSpan(8, 0, 0).Add(TimeSpan.FromDays(1)))
                {
                    shiftRate = Type2Rate * 24;
                }

                totalSalary += shiftRate;
            }

            return totalSalary;
        }

        /// <summary>
        /// Retrieves all doctors from the database.
        /// </summary>
        /// <returns>A list of all doctors.</returns>
        public List<DoctorJointModel> GetDoctors()
        {
            List<DoctorJointModel> doctors = new List<DoctorJointModel>();
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT * FROM Doctors";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    doctors.Add(new DoctorJointModel(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetInt32(4),
                        reader.GetDouble(3),
                        reader.GetString(5)));
                }
            }

            return doctors;
        }
    }
}
