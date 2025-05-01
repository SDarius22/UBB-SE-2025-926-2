namespace Hospital.DatabaseServices
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Numerics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using Hospital.Configs;
    using Hospital.DbContext;
    using Hospital.Exceptions;
    using Hospital.Models;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Windows.System;
    using DoctorJointModel = Hospital.Models.DoctorJointModel;

    public class DoctorsDatabaseService : IDoctorsDatabaseService
    {

        private const double Type0Rate = 200d;
        private const double Type1Rate = Type0Rate * 1.2d;
        private const double Type2Rate = Type1Rate * 1.5d;

        private readonly ApplicationConfiguration _configuration;
        private readonly AppDbContext _context;

        public DoctorsDatabaseService(AppDbContext context)
        {
            this._configuration = ApplicationConfiguration.GetInstance();
            _context = context;
        }


        // This method will be used to get the doctors from the database
        public async Task<List<DoctorJointModel>> GetDoctorsByDepartment(int departmentId)
        {
            try
            {
                var query = FormattableStringFactory.Create(@"
                    SELECT
                        d.DoctorId,
                        d.UserId,
                        u.Username,
                        d.DepartmentId,
                        d.Rating,
                        d.LicenseNumber
                    FROM Doctors d
                    INNER JOIN Users u ON d.UserId = u.UserId
                    WHERE d.DepartmentId = {0}",
                            departmentId);

                var doctors = await Task.Run(() =>
                    _context.Database.SqlQuery<DoctorJointModel>(query).ToList());

                return doctors;
            }
            catch (SqlException sqlException)
            {
                return new List<DoctorJointModel>();
            }
            catch (Exception exception)
            {
                return new List<DoctorJointModel>();
            }
        }

        /// <summary>
        /// Adds a new doctor to the database.
        /// </summary>
        /// <param name="doctor">The doctor to add.</param>
        /// <returns>True if the doctor was added successfully; otherwise, false.</returns>
        public async Task<bool> AddDoctor(DoctorJointModel doctor)
        {
            try
            {
                var query = FormattableStringFactory.Create(
                    "INSERT INTO Doctors (UserID, DepartmentID, Rating, LicenseNumber) " +
                    "VALUES ({0}, {1}, {2}, {3})",
                    doctor.UserId,
                    doctor.DepartmentId,
                    doctor.DoctorRating,
                    doctor.LicenseNumber);

                var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(query);
                return rowsAffected > 0;
            }
            catch (SqlException sqlException)
            {
                throw new DatabaseOperationException($"SQL Error: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException($"General Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Updates an existing doctor's details in the database.
        /// </summary>
        /// <param name="doctor">The doctor with updated details.</param>
        /// <returns>True if the doctor was updated successfully; otherwise, false.</returns>
        public async Task<bool> UpdateDoctor(DoctorJointModel doctor)
        {
            try
            {
                var query = FormattableStringFactory.Create(
                    "UPDATE Doctors SET " +
                    "UserID = {0}, " +
                    "DepartmentID = {1}, " +
                    "LicenseNumber = {2} " +
                    "WHERE DoctorID = {3}",
                    doctor.UserId,
                    doctor.DepartmentId,
                    doctor.LicenseNumber,
                    doctor.DoctorId);

                var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(query);
                return rowsAffected > 0;
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
        public async Task<bool> DeleteDoctor(int doctorID)
        {
            try
            {
                var query = FormattableStringFactory.Create(
                    "DELETE FROM Doctors WHERE DoctorID = {0}",
                    doctorID);

                var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(query);
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseOperationException($"SQL Error deleting doctor: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Error deleting doctor: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a doctor exists in the database based on the provided doctor ID.
        /// </summary>
        /// <param name="doctorID">The ID of the doctor to check.</param>
        /// <returns>True if the doctor exists; otherwise, false.</returns>
        public async Task<bool> DoesDoctorExist(int doctorID)
        {
            try
            {
                var query = FormattableStringFactory.Create(
                    "SELECT COUNT(*) FROM Doctors WHERE DoctorID = {0}",
                    doctorID);

                var count = await Task.Run(() =>
                    _context.Database.SqlQuery<int>(query).FirstOrDefault());

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseOperationException($"SQL Error checking doctor existence: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Error checking doctor existence: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a user is already a doctor in the database.
        /// </summary>
        /// <param name="userID">The ID of the user to check.</param>
        /// <returns>True if the user is already a doctor; otherwise, false.</returns>
        public async Task<bool> IsUserAlreadyDoctor(int userID)
        {
            try
            {
                var query = FormattableStringFactory.Create(
                    "SELECT COUNT(*) FROM Doctors WHERE UserID = {0}",
                    userID);

                // Using Task.Run to wrap the synchronous operation
                var count = await Task.Run(() =>
                    _context.Database.SqlQuery<int>(query).FirstOrDefault());

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseOperationException($"SQL Error checking doctor status: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Error checking doctor status: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a user exists in the database.
        /// </summary>
        /// <param name="userID">The ID of the user to check.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        public async Task<bool> DoesUserExist(int userID)
        {
            try
            {
                return await _context.Users
                    .AnyAsync(u => u.UserId == userID);
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Error checking user existence: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a user has the role of a doctor.
        /// </summary>
        /// <param name="userID">The ID of the user to check.</param>
        /// <returns>True if the user is a doctor; otherwise, false.</returns>
        public async Task<bool> IsUserDoctor(int userID)
        {
            try
            {
                var query = FormattableStringFactory.Create(
                    "SELECT Role FROM Users WHERE UserID = {0}",
                    userID);

                // Using Task.Run to wrap the synchronous operation
                var role = await Task.Run(() =>
                    _context.Database.SqlQuery<string>(query).FirstOrDefault());

                return role == "Doctor";
            }
            catch (SqlException ex)
            {
                throw new DatabaseOperationException($"SQL Error checking user role: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Error checking user role: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a department exists in the database.
        /// </summary>
        /// <param name="departmentID">The ID of the department to check.</param>
        /// <returns>True if the department exists; otherwise, false.</returns>
        public async Task<bool> DoesDepartmentExist(int departmentID)
        {
            try
            {
                var query = FormattableStringFactory.Create(
                    "SELECT COUNT(*) FROM Departments WHERE DepartmentID = {0}",
                    departmentID);

                // Using Task.Run to wrap the synchronous operation
                var count = await Task.Run(() =>
                    _context.Database.SqlQuery<int>(query).FirstOrDefault());

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseOperationException($"SQL Error checking department existence: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Error checking department existence: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a user exists in the doctors table but with a different doctor ID.
        /// </summary>
        /// <param name="userID">The ID of the user to check.</param>
        /// <param name="doctorID">The ID of the doctor to exclude from the check.</param>
        /// <returns>True if the user exists in the doctors table with a different doctor ID; otherwise, false.</returns>
        public async Task<bool> UserExistsInDoctors(int userID, int doctorID)
        {
            try
            {
                // Create parameterized query using FormattableStringFactory
                FormattableString query = FormattableStringFactory.Create(
                    "SELECT COUNT(*) FROM Doctors WHERE UserID = {0} AND DoctorID <> {1}",
                    userID,
                    doctorID);

                // Execute query asynchronously
                int count = await _context.Database
                    .SqlQuery<int>(query)
                    .FirstOrDefaultAsync();

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseOperationException($"SQL Error checking doctor assignment: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Error checking doctor assignment: {ex.Message}");
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
                        DateOnly.FromDateTime(reader.GetDateTime(1)), // Convert DateTime to DateOnly
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
        public async Task<double> ComputeDoctorSalary(int doctorID)
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
        public async Task<List<DoctorJointModel>> GetDoctors()
        {
            List<DoctorJointModel> doctors = new List<DoctorJointModel>();
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT DoctorId, UserId, DepartmentId, Rating, LicenseNumber FROM Doctors";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    doctors.Add(new DoctorJointModel(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetInt32(2),
                        reader.GetDouble(3),
                        reader.GetString(4)));
                }
            }

            return doctors;
        }
    }
}