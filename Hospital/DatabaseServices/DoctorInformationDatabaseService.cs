using Hospital.Configs;
using Hospital.DatabaseServices.Interfaces;
using Hospital.DbContext;
using Hospital.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DoctorInformationModel = Hospital.Models.DoctorInformationModel;

namespace Hospital.DatabaseServices
{
    public class DoctorInformationDatabaseService : IDoctorInformationDatabaseService
    {
        private readonly ApplicationConfiguration _configuration;
        private readonly AppDbContext _context;

        public DoctorInformationDatabaseService(AppDbContext context)
        {
            this._configuration = ApplicationConfiguration.GetInstance();
            _context = context;
        }

        public async Task<DoctorInformationModel> GetDoctorInformation(int doctorId)
        {
            try
            {
                var query = FormattableStringFactory.Create(@"
                    SELECT 
                        UserID, Username, Mail, Role, Name, Birthdate, Cnp, Address, 
                        PhoneNumber, RegistrationDate, DoctorID, LicenseNumber, 
                        Experience, Rating, DepartmentID, Name as DepartmentName
                    FROM UserDoctorDepartmentView
                    WHERE DoctorID = {0}",
                    doctorId);

                var doctorInfo = await Task.Run(() =>
                    _context.Database.SqlQuery<DoctorInformationModel>(query).FirstOrDefault());

                return doctorInfo ?? throw new DatabaseOperationException("Doctor not found");
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
        /// Computes the salary of a doctor based on their shifts in the current month.
        /// </summary>
        /// <param name="doctorId">The unique identifier of the doctor.</param>
        /// <returns>The computed salary as a decimal value.</returns>
        public async Task<decimal> ComputeSalary(int doctorId)
        {
            decimal salary = 0;

            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                connection.Open();
                string query = @"
                            SELECT StartTime, EndTime
                            FROM GetCurrentMonthShiftsForDoctor(@DoctorID)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DoctorID", doctorId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TimeSpan startTime = reader.GetTimeSpan(reader.GetOrdinal("StartTime"));
                            TimeSpan endTime = reader.GetTimeSpan(reader.GetOrdinal("EndTime"));

                            if (startTime == new TimeSpan(8, 0, 0) && endTime == new TimeSpan(20, 0, 0))
                            {
                                salary += 100 * 12;
                            }
                            else if (startTime == new TimeSpan(20, 0, 0) && endTime == new TimeSpan(8, 0, 0))
                            {
                                salary += 100 * 1.2m * 12;
                            }
                            else if (startTime == new TimeSpan(8, 0, 0) && endTime == new TimeSpan(8, 0, 0))
                            {
                                salary += 100 * 1.5m * 24;
                            }
                        }
                    }
                }
            }

            return salary;
        }
    }
}