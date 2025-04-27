using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Hospital.Managers
{
    public interface IShiftManager
    {
        Task LoadShifts(int doctorID);
        List<ShiftModel> GetShifts();
        ShiftModel GetShiftByDay(DateTime day);
        Task LoadUpcomingDoctorDayshifts(int doctorID);
        List<TimeSlotModel> GenerateTimeSlots(DateTime date, List<ShiftModel> shifts, List<AppointmentJointModel> appointments);
        (DateTimeOffset start, DateTimeOffset end) GetMonthlyCalendarRange();
    }
}
