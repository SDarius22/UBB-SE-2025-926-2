namespace Backend.DatabaseServices
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
    using Backend.Configs;
    using Backend.DbContext;
    using Backend.Exceptions;
    using Backend.Models;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using DoctorJointModel = Backend.Models.DoctorJointModel;

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
                return await _context.DoctorJoints
                    .Join(
                        _context.Users,
                        d => d.UserId,
                        u => u.UserID,
                        (d, u) => new DoctorJointModel
                        {
                            DoctorId = d.DoctorId,
                            UserId = d.UserId,
                            DepartmentId = d.DepartmentId,
                            Rating = d.Rating,
                            LicenseNumber = d.LicenseNumber
                        }
                    )
                    .Where(d => d.DepartmentId == departmentId)
                    .ToListAsync();
            }
            catch (SqlException)
            {
                return new List<DoctorJointModel>();
            }
            catch (Exception)
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
                var doctorEntity = new DoctorJointModel
                {
                    UserId = doctor.UserId,
                    DepartmentId = doctor.DepartmentId,
                    Rating = doctor.Rating,
                    LicenseNumber = doctor.LicenseNumber
                };

                await _context.DoctorJoints.AddAsync(doctorEntity);
                var rowsAffected = await _context.SaveChangesAsync();
                return rowsAffected > 0;
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseOperationException($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"General Error: {ex.Message}");
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
                var existingDoctor = await _context.DoctorJoints
                    .FirstOrDefaultAsync(d => d.DoctorId == doctor.DoctorId);

                if (existingDoctor == null)
                {
                    return false;
                }

                existingDoctor.UserId = doctor.UserId;
                existingDoctor.DepartmentId = doctor.DepartmentId;
                existingDoctor.LicenseNumber = doctor.LicenseNumber;

                var rowsAffected = await _context.SaveChangesAsync();
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
                var doctor = await _context.DoctorJoints
                    .FirstOrDefaultAsync(d => d.DoctorId == doctorID);

                if (doctor == null)
                {
                    return false;
                }

                _context.DoctorJoints.Remove(doctor);
                var rowsAffected = await _context.SaveChangesAsync();
                return rowsAffected > 0;
            }
            catch (DbUpdateException ex)
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
                return await _context.DoctorJoints
                    .AnyAsync(d => d.DoctorId == doctorID);
            }
            catch (DbUpdateException ex)
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
                return await _context.DoctorJoints
                    .AnyAsync(d => d.UserId == userID);
            }
            catch (DbUpdateException ex)
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
                    .AnyAsync(u => u.UserID == userID);
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

                var role = await _context.Users
                    .Where(u => u.UserID == userID)
                    .Select(u => u.Role)
                    .FirstOrDefaultAsync();

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
                return await _context.Departments
                    .AnyAsync(d => d.DepartmentID == departmentID);
            }
            catch (DbUpdateException ex)
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
                return await _context.DoctorJoints
                    .AnyAsync(d => d.UserId == userID && d.DoctorId != doctorID);
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseOperationException($"SQL Error checking doctor assignment: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Error checking doctor assignment: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all doctors from the database.
        /// </summary>
        /// <returns>A list of all doctors.</returns>
        public async Task<List<DoctorJointModel>> GetDoctors()
        {
            try
            {
                return await _context.DoctorJoints
                    .Select(d => new DoctorJointModel(
                        d.DoctorId,
                        d.UserId,
                        d.DepartmentId,
                        d.Rating,
                        d.LicenseNumber))
                    .ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseOperationException($"SQL Error retrieving doctors: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Error retrieving doctors: {ex.Message}");
            }
        }
    }
}