using Hospital.Configs;
using Hospital.DatabaseServices.Interfaces;
using Hospital.DbContext;
using Hospital.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDatabaseService"/> class.
        /// </summary>
        public ScheduleDatabaseService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Function to add a schedule to the database.
        /// </summary>
        /// <param name="schedule">The schedule to add.</param>
        /// <returns>True if the schedule was added successfully, false otherwise.</returns>
        public async Task<bool> AddSchedule(ScheduleModel schedule)
        {
            try
            {
                var entity = new ScheduleModel(schedule.ScheduleId, schedule.DoctorId, schedule.ShiftId);

                await _context.Schedules.AddAsync(entity);
                await _context.SaveChangesAsync();

                schedule.ScheduleId = entity.ScheduleId;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Function to update a schedule in the database.
        /// </summary>
        /// <param name="schedule">The schedule to update.</param>
        /// <returns>True if the schedule was updated successfully, false otherwise.</returns>
        public async Task<bool> UpdateSchedule(ScheduleModel schedule)
        {
            try
            {
                var existingSchedule = await _context.Schedules.FindAsync(schedule.ScheduleId);
                if (existingSchedule == null)
                {
                    return false;
                }

                existingSchedule.DoctorId = schedule.DoctorId;
                existingSchedule.ShiftId = schedule.ShiftId;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Function to delete a schedule from the database.
        /// </summary>
        /// <param name="scheduleID">The ID of the schedule to delete.</param>
        /// <returns>True if the schedule was deleted successfully, false otherwise.</returns>
        public async Task<bool> DeleteSchedule(int scheduleId)
        {
            try
            {
                var schedule = await _context.Schedules.FindAsync(scheduleId);
                if (schedule == null) return false;

                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Function to check if a schedule exists in the database.
        /// </summary>
        /// <param name="scheduleID">The ID of the schedule to check.</param>
        /// <returns>True if the schedule exists, false otherwise.</returns>
        public async Task<bool> DoesScheduleExist(int scheduleId)
        {
            return await _context.Schedules
                .AnyAsync(s => s.ScheduleId == scheduleId);
        }


        /// <summary>
        /// Function to check if a doctor exists in the database.
        /// </summary>
        /// <param name="doctorID">The ID of the doctor to check.</param>
        /// <returns>True if the doctor exists, false otherwise.</returns>
        public async Task<bool> DoesDoctorExist(int doctorId)
        {
            return await _context.DoctorJoints
                .AnyAsync(d => d.DoctorId == doctorId);
        }

        /// <summary>
        /// Function to check if a shift exists in the database.
        /// </summary>
        /// <param name="shiftID">The ID of the shift to check.</param>
        /// <returns>True if the shift exists, false otherwise.</returns>
        public async Task<bool> DoesShiftExist(int shiftId)
        {
            return await _context.Shifts
                .AnyAsync(s => s.ShiftID == shiftId);
        }

        /// <summary>
        /// Function to get a list of all schedules from the database.
        /// </summary>
        /// <returns>A list of schedules from the database.</returns>
        public async Task<List<ScheduleModel>> GetSchedules()
        {
            try
            {
                return await _context.Schedules
                    .Select(s => new ScheduleModel(
                        s.ScheduleId,
                        s.DoctorId,
                        s.ShiftId
                    ))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ShiftNotFoundException($"Error loading schedules: {ex.Message}");
            }
        }
    }
}