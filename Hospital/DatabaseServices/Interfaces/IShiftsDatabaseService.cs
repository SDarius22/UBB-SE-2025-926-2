using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public interface IShiftsDatabaseService
    {
        Task<List<ShiftModel>> GetShifts();
        Task<List<ScheduleModel>> GetSchedules();
        Task<List<ShiftModel>> GetShiftsByDoctorId(int doctorId);
        Task<List<ShiftModel>> GetDoctorDaytimeShifts(int doctorId);

        public bool AddShift(ShiftModel shift);

        public bool UpdateShift(ShiftModel shift);

        public bool DoesShiftExist(int shiftID);

        public bool DeleteShift(int shiftID);
    }
}