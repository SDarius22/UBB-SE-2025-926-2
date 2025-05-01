using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.DatabaseServices
{
    public interface IShiftsDatabaseService
    {
        Task<List<ShiftModel>> GetShifts();
        Task<List<ScheduleModel>> GetSchedules();
        Task<List<ShiftModel>> GetShiftsByDoctorId(int doctorId);
        Task<List<ShiftModel>> GetDoctorDaytimeShifts(int doctorId);

        public Task<bool> AddShift(ShiftModel shift);

        public Task<bool> UpdateShift(ShiftModel shift);

        public Task<bool> DoesShiftExist(int shiftID);

        public Task<bool> DeleteShift(int shiftID);
    }
}