using Hospital.Configs;
using Hospital.DbContext;
using Hospital.Exceptions;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public class ShiftsDatabaseService : IShiftsDatabaseService
    {
        private readonly AppDbContext _context;

        public ShiftsDatabaseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ShiftModel>> GetShifts()
        {
            try
            {
                return await _context.Shifts
                    .Select(s => new ShiftModel(
                        s.ShiftId,
                        s.Date,
                        s.StartTime,
                        s.EndTime
                    )).ToListAsync();
            }
            catch (Exception exception)
            {
                throw new ShiftNotFoundException($"Error loading shifts: {exception.Message}");
            }
        }

        public async Task<List<ScheduleModel>> GetSchedules()
        {
            try
            {
                return await _context.Schedules
                    .Select(s => new ScheduleModel(
                        s.DoctorId,
                        s.ShiftId,
                        s.ScheduleId
                    ))
                    .ToListAsync();
            }
            catch (Exception exception)
            {
                throw new ShiftNotFoundException($"Error loading schedules: {exception.Message}");
            }
        }

        public async Task<List<ShiftModel>> GetShiftsByDoctorId(int doctorId)
        {
            if (doctorId <= 0)
            {
                throw new ShiftNotFoundException("Invalid doctor ID");
            }

            try
            {
                return await _context.Schedules
                    .Where(s => s.DoctorId == doctorId)
                    .Join(_context.Shifts,
                        schedule => schedule.ShiftId,
                        shift => shift.ShiftId,
                        (schedule, shift) => new ShiftModel(
                            shift.ShiftId,
                            shift.Date,
                            shift.StartTime,
                            shift.EndTime
                        ))
                    .ToListAsync();
            }
            catch (Exception exception)
            {
                throw new ShiftNotFoundException($"Error loading shifts for doctor {doctorId}: {exception.Message}");
            }
        }

        public async Task<List<ShiftModel>> GetDoctorDaytimeShifts(int doctorId)
        {
            if (doctorId <= 0)
            {
                throw new ShiftNotFoundException("Invalid doctor ID");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            try
            {
                return await _context.Schedules
                    .Where(s => s.DoctorId == doctorId)
                    .Join(_context.Shifts,
                        schedule => schedule.ShiftId,
                        shift => shift.ShiftId,
                        (schedule, shift) => new { schedule, shift })
                    .Where(x => x.shift.StartTime < TimeSpan.FromHours(20) &&
                                x.shift.Date >= today)
                    .Select(x => new ShiftModel(
                        x.shift.ShiftId,
                        x.shift.Date,
                        x.shift.StartTime,
                        x.shift.EndTime
                    ))
                    .ToListAsync();
            }
            catch (Exception exception)
            {
                throw new ShiftNotFoundException($"Error loading daytime shifts: {exception.Message}");
            }
        }

        public async Task<bool> AddShift(ShiftModel shift)
        {
            try
            {
                var entity = new ShiftModel(shift.ShiftId, shift.Date, shift.StartTime, shift.EndTime);

                _context.Shifts.Add(entity);
                await _context.SaveChangesAsync();

                shift.ShiftId = entity.ShiftId;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateShift(ShiftModel shift)
        {
            try
            {
                var existingShift = await this._context.Shifts.FindAsync(shift.ShiftId);
                if (existingShift == null)
                {
                    return false;
                }

                existingShift.Date = shift.Date;
                existingShift.StartTime = shift.StartTime;
                existingShift.EndTime = shift.EndTime;

                await this._context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DoesShiftExist(int shiftId)
        {
            return await _context.Shifts
                .AnyAsync(s => s.ShiftId == shiftId);
        }

        public async Task<bool> DeleteShift(int shiftId)
        {
            try
            {
                var shift = await _context.Shifts.FindAsync(shiftId);
                if (shift == null) return false;

                _context.Shifts.Remove(shift);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}