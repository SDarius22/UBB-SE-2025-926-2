using Hospital.Configs;
using Hospital.Exceptions;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public class ShiftsDatabaseService : IShiftsDatabaseService
    {
        private readonly ApplicationConfiguration _configuration;

        public ShiftsDatabaseService()
        {
            this._configuration = ApplicationConfiguration.GetInstance();
        }

        public async Task<List<ShiftModel>> GetShifts()
        {
            const string selectShiftsQuery = "SELECT ShiftId, Date, StartTime, EndTime FROM Shifts";
            List<ShiftModel> shifts = new List<ShiftModel>();

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(_configuration.DatabaseConnection);
                await sqlConnection.OpenAsync();

                using SqlCommand selectShiftsCommand = new SqlCommand(selectShiftsQuery, sqlConnection);
                using SqlDataReader reader = await selectShiftsCommand.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    shifts.Add(new ShiftModel(
                        reader.GetInt32(0),
                        DateOnly.FromDateTime(reader.GetDateTime(1)),
                        reader.GetTimeSpan(2),
                        reader.GetTimeSpan(3)
                    ));
                }
            }
            catch (SqlException sqlException)
            {
                throw new ShiftNotFoundException($"Database error loading shifts: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new ShiftNotFoundException($"Error loading shifts: {exception.Message}");
            }

            return shifts;
        }

        public async Task<List<ScheduleModel>> GetSchedules()
        {
            const string selectSchedulesQuery = "SELECT DoctorId, ShiftId, ScheduleId FROM Schedules";
            List<ScheduleModel> schedules = new List<ScheduleModel>();

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(_configuration.DatabaseConnection);
                await sqlConnection.OpenAsync();

                using SqlCommand selectSchedulesCommand = new SqlCommand(selectSchedulesQuery, sqlConnection);

                using SqlDataReader reader = await selectSchedulesCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    schedules.Add(new ScheduleModel(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2)));
                }
            }
            catch (SqlException sqlException)
            {
                throw new ShiftNotFoundException($"SQL Error: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new ShiftNotFoundException($"General Error: {exception.Message}");
            }

            return schedules;
        }

        public async Task<List<ShiftModel>> GetShiftsByDoctorId(int doctorId)
        {
            if (doctorId <= 0)
            {
                throw new ShiftNotFoundException("Error loading shifts for doctor.");
            }
            const string selectShiftsByDoctorIdQuery = @"
            SELECT s.ShiftId, s.Date, s.StartTime, s.EndTime
            FROM Shifts s
            JOIN Schedules sch ON s.ShiftId = sch.ShiftId
            WHERE sch.DoctorId = @DoctorId";

            List<ShiftModel> shifts = new List<ShiftModel>();

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(_configuration.DatabaseConnection);
                await sqlConnection.OpenAsync();

                using SqlCommand selectShiftsByDoctorIdCommand = new SqlCommand(selectShiftsByDoctorIdQuery, sqlConnection);
                selectShiftsByDoctorIdCommand.Parameters.AddWithValue("@DoctorId", doctorId);

                using SqlDataReader reader = await selectShiftsByDoctorIdCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    shifts.Add(new ShiftModel(
                        reader.GetInt32(0),
                        DateOnly.FromDateTime(reader.GetDateTime(1)),
                        reader.GetTimeSpan(2),
                        reader.GetTimeSpan(3)
                    ));
                }
            }
            catch (SqlException sqlException)
            {
                throw new ShiftNotFoundException($"Database error loading shifts for doctor {doctorId}: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new ShiftNotFoundException($"Error loading shifts for doctor {doctorId}");
            }

            return shifts;
        }

        public async Task<List<ShiftModel>> GetDoctorDaytimeShifts(int doctorId)
        {
            if (doctorId <= 0)
            {
                throw new ShiftNotFoundException("Error loading upcoming shifts for doctor.");
            }
            const string selectDaytimeShiftByDoctorIdQuery = @"
            SELECT s.ShiftId, s.Date, s.StartTime, s.EndTime
            FROM Shifts s
            JOIN Schedules sch ON s.ShiftId = sch.ShiftId
            WHERE sch.DoctorId = @DoctorId AND s.StartTime < '20:00:00'
            AND CAST(s.DateTime AS DATE) >= CAST(GETDATE() AS DATE)";

            List<ShiftModel> shifts = new List<ShiftModel>();

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(_configuration.DatabaseConnection);
                await sqlConnection.OpenAsync();

                using SqlCommand selectDaytimeShiftsByDoctorIdCommand = new SqlCommand(selectDaytimeShiftByDoctorIdQuery, sqlConnection);
                selectDaytimeShiftsByDoctorIdCommand.Parameters.AddWithValue("@DoctorId", doctorId);

                using SqlDataReader reader = await selectDaytimeShiftsByDoctorIdCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    shifts.Add(new ShiftModel(
                        reader.GetInt32(0),
                        DateOnly.FromDateTime(reader.GetDateTime(1)),
                        reader.GetTimeSpan(2),
                        reader.GetTimeSpan(3)
                    ));
                }
            }
            catch (SqlException sqlException)
            {
                throw new ShiftNotFoundException($"Database error loading upcoming shifts for doctor {doctorId}: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new ShiftNotFoundException($"Error loading upcoming shifts for doctor {doctorId}: {exception.Message}");
            }

            return shifts;
        }

        public bool AddShift(ShiftModel shift)
        {
            using SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection);
            string query = "INSERT INTO Shifts (Date, StartTime, EndTime) VALUES (@Date, @StartTime, @EndTime)";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Date", shift.Date.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@StartTime", shift.StartTime);
            command.Parameters.AddWithValue("@EndTime", shift.EndTime);

            connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool UpdateShift(ShiftModel shift)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection);
                string query = "UPDATE Shifts SET Date = @Date, StartTime = @StartTime, EndTime = @EndTime WHERE ShiftId = @ShiftId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Date", shift.Date.ToDateTime(TimeOnly.MinValue));
                command.Parameters.AddWithValue("@StartTime", shift.StartTime);
                command.Parameters.AddWithValue("@EndTime", shift.EndTime);
                command.Parameters.AddWithValue("@ShiftId", shift.ShiftId);

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

        public bool DoesShiftExist(int shiftID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT COUNT(*) FROM Shifts WHERE ShiftId = @ShiftId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ShiftId", shiftID);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        public bool DeleteShift(int shiftID)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "DELETE FROM Shifts WHERE ShiftId = @ShiftID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ShiftId", shiftID);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}