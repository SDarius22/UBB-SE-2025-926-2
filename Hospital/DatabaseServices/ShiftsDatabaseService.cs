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
                        reader.GetDateTime(1),
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
            const string selectSchedulesQuery = "SELECT DoctorId, ShiftId FROM Schedules";
            List<ScheduleModel> schedules = new List<ScheduleModel>();

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(_configuration.DatabaseConnection);
                await sqlConnection.OpenAsync();

                using SqlCommand selectSchedulesCommand = new SqlCommand(selectSchedulesQuery, sqlConnection);

                using SqlDataReader reader = await selectSchedulesCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    schedules.Add(new ScheduleModel(reader.GetInt32(0), reader.GetInt32(1)));
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
                        reader.GetDateTime(1),
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
            if(doctorId <= 0)
            {
                throw new ShiftNotFoundException("Error loading upcoming shifts for doctor.");
            }
            const string selectDaytimeShiftByDoctorIdQuery = @"
            SELECT s.ShiftId, s.Date, s.StartTime, s.EndTime
            FROM Shifts s
            JOIN Schedules sch ON s.ShiftId = sch.ShiftId
            WHERE sch.DoctorId = @DoctorId AND s.StartTime < '20:00:00'
            AND CAST(s.Date AS DATE) >= CAST(GETDATE() AS DATE)";

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
                        reader.GetDateTime(1),
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
    }


}

