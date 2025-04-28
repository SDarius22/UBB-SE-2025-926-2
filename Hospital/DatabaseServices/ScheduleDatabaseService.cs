using Hospital.Configs;
using Hospital.DatabaseServices.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleModel = Hospital.Models.ScheduleModel;

namespace Hospital.DatabaseServices
{
    public class ScheduleDatabaseService : IScheduleDatabaseService
    {

        private readonly ApplicationConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDatabaseService"/> class.
        /// </summary>
        public ScheduleDatabaseService()
        {
            this._configuration = ApplicationConfiguration.GetInstance();
        }

        /// <summary>
        /// Function to add a schedule to the database.
        /// </summary>
        /// <param name="schedule">The schedule to add.</param>
        /// <returns>True if the schedule was added successfully, false otherwise.</returns>
        public bool AddSchedule(ScheduleModel schedule)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "INSERT INTO Schedules (DoctorId, ShiftId) VALUES (@DoctorID, @ShiftID)";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@DoctorID", schedule.DoctorId);
                command.Parameters.AddWithValue("@ShiftID", schedule.ShiftId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Function to update a schedule in the database.
        /// </summary>
        /// <param name="schedule">The schedule to update.</param>
        /// <returns>True if the schedule was updated successfully, false otherwise.</returns>
        public bool UpdateSchedule(ScheduleModel schedule)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection);
                string query = "UPDATE Schedules SET DoctorId = @DoctorID, ShiftId = @ShiftID WHERE ScheduleId = @ScheduleID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DoctorID", schedule.DoctorId);
                command.Parameters.AddWithValue("@ShiftID", schedule.ShiftId);
                command.Parameters.AddWithValue("@ScheduleID", schedule.ScheduleId);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"SQL Error: {exception.Message}");
                return false;
            }
            catch (InvalidOperationException exception)
            {
                Console.WriteLine($"Invalid Operation: {exception.Message}");
                return false;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Unexpected Error: {exception.Message}");
                return false;
            }
        }

        /// <summary>
        /// Function to delete a schedule from the database.
        /// </summary>
        /// <param name="scheduleID">The ID of the schedule to delete.</param>
        /// <returns>True if the schedule was deleted successfully, false otherwise.</returns>
        public bool DeleteSchedule(int scheduleID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "DELETE FROM Schedules WHERE ScheduleId = @ScheduleID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ScheduleID", scheduleID);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Function to check if a schedule exists in the database.
        /// </summary>
        /// <param name="scheduleID">The ID of the schedule to check.</param>
        /// <returns>True if the schedule exists, false otherwise.</returns>
        public bool DoesScheduleExist(int scheduleID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT COUNT(*) FROM Schedules WHERE ScheduleId = @ScheduleID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ScheduleID", scheduleID);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Function to check if a doctor exists in the database.
        /// </summary>
        /// <param name="doctorID">The ID of the doctor to check.</param>
        /// <returns>True if the doctor exists, false otherwise.</returns>
        public bool DoesDoctorExist(int doctorID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT COUNT(*) FROM Doctors WHERE DoctorId = @DoctorID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DoctorID", doctorID);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Function to check if a shift exists in the database.
        /// </summary>
        /// <param name="shiftID">The ID of the shift to check.</param>
        /// <returns>True if the shift exists, false otherwise.</returns>
        public bool DoesShiftExist(int shiftID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT COUNT(*) FROM Shifts WHERE ShiftId = @ShiftID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ShiftID", shiftID);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Function to get a list of all schedules from the database.
        /// </summary>
        /// <returns>A list of schedules from the database.</returns>
        public List<ScheduleModel> GetSchedules()
        {
            List<ScheduleModel> schedules = new List<ScheduleModel>();
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT ScheduleId, DoctorId, ShiftId FROM Schedules";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    schedules.Add(new ScheduleModel(
                        reader.GetInt32(reader.GetOrdinal("ScheduleId")),
                        reader.GetInt32(reader.GetOrdinal("DoctorId")),
                        reader.GetInt32(reader.GetOrdinal("ShiftId"))));
                }
            }

            return schedules;
        }
    }
}