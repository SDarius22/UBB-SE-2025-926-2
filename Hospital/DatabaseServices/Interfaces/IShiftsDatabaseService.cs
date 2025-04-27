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
    }
}
