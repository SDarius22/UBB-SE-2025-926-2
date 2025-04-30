using Hospital.Configs;
using Hospital.DatabaseServices.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoctorInformationModel = Hospital.Models.DoctorInformationModel;

namespace Hospital.DatabaseServices
{
    public class DoctorInformationDatabaseService : IDoctorInformationDatabaseService
    {
        private readonly ApplicationConfiguration _configuration;

        public DoctorInformationDatabaseService()
        {
            this._configuration = ApplicationConfiguration.GetInstance();
        }

        public DoctorInformationModel GetDoctorInformation(int doctorId)
        {
            DoctorInformationModel? doctorInformationModel = null; // Use nullable type

            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                connection.Open();
                string query = @"
                            SELECT 
                                UserID, Username, Mail, Role, Name, Birthdate, Cnp, Address, PhoneNumber, RegistrationDate, 
                                DoctorID, LicenseNumber, Experience, Rating, DepartmentID, Name
                            FROM UserDoctorDepartmentView
                            WHERE DoctorID = @DoctorID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DoctorID", doctorId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            doctorInformationModel = new DoctorInformationModel(
                                reader.GetInt32(reader.GetOrdinal("UserID")),
                                reader.GetString(reader.GetOrdinal("Username")),
                                reader.GetString(reader.GetOrdinal("Mail")),
                                reader.GetString(reader.GetOrdinal("Role")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetDateTime(reader.GetOrdinal("Birthdate")),
                                reader.GetString(reader.GetOrdinal("Cnp")),
                                reader.GetString(reader.GetOrdinal("Address")),
                                reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                reader.GetDateTime(reader.GetOrdinal("RegistrationDate")),
                                reader.GetInt32(reader.GetOrdinal("DoctorID")),
                                reader.GetString(reader.GetOrdinal("LicenseNumber")),
                                (float)reader.GetDouble(reader.GetOrdinal("Rating")),
                                reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                reader.GetString(reader.GetOrdinal("DepartmentName")));
                        }
                        else
                        {
                            throw new Exception("Doctor not found");
                        }
                    }
                }
            }

            return doctorInformationModel ?? throw new Exception("Doctor information is null");
        }

        /// <summary>
        /// Computes the salary of a doctor based on their shifts in the current month.
        /// </summary>
        /// <param name="doctorId">The unique identifier of the doctor.</param>
        /// <returns>The computed salary as a decimal value.</returns>
        public decimal ComputeSalary(int doctorId)
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