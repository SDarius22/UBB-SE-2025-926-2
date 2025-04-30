using Hospital.DatabaseServices;
using System;
using Hospital.Exceptions;
using Hospital.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;

namespace Hospital.Managers
{
    public class ShiftManager : IShiftManager
    {
        private readonly IShiftsDatabaseService _shiftsDatabaseService;
        private List<ShiftModel> _shifts;

        public ShiftManager(IShiftsDatabaseService shiftsDatabaseService)
        {
            _shiftsDatabaseService = shiftsDatabaseService;
            _shifts = new List<ShiftModel>();
        }

        public async Task LoadShifts(int doctorID)
        {
            _shifts = await _shiftsDatabaseService.GetShiftsByDoctorId(doctorID);
        }

        public List<ShiftModel> GetShifts()
        {
            return _shifts;
        }

        public ShiftModel GetShiftByDay(DateOnly day)
        {
            ShiftModel? shiftByDate = _shifts.FirstOrDefault(shift => shift.Date == day);
            if (shiftByDate == null)
                throw new ShiftNotFoundException(string.Format("Shift not found for date {0}", day.ToString()));
            return shiftByDate;
        }

        public async Task LoadUpcomingDoctorDayshifts(int doctorID)
        {
            _shifts = await _shiftsDatabaseService.GetDoctorDaytimeShifts(doctorID);
        }

        public (DateTimeOffset start, DateTimeOffset end) GetMonthlyCalendarRange()
        {
            var today = DateTime.Today;
            var start = new DateTimeOffset(new DateTime(today.Year, today.Month, 1));
            var end = start.AddMonths(1).AddDays(-1);
            return (start, end);
        }

        public List<TimeSlotModel> GenerateTimeSlots(DateOnly date, List<ShiftModel> shifts, List<AppointmentJointModel> appointments)
        {
            var timeSlots = new List<TimeSlotModel>();
            var shift = shifts.FirstOrDefault(s => s.Date == date);

            if (shift == null)
                return timeSlots;

            var currentTime = shift.StartTime;
            var endTime = shift.EndTime;

            if (endTime < currentTime)
                endTime = endTime.Add(TimeSpan.FromDays(1));

            while (currentTime < endTime)
            {
                var slotEndTime = currentTime.Add(TimeSpan.FromMinutes(30));
                if (slotEndTime > endTime)
                    slotEndTime = endTime;

                var appointment = appointments.FirstOrDefault(a =>
                    a.DateAndTime.TimeOfDay >= currentTime &&
                    a.DateAndTime.TimeOfDay < slotEndTime);

                timeSlots.Add(new TimeSlotModel
                {
                    Appointment = appointment != null ? $"{appointment.PatientName} - {appointment.ProcedureName}" : string.Empty,
                    HighlightStatus = appointment != null ? "Appointment" : "None"
                });

                currentTime = slotEndTime;
            }

            return timeSlots;
        }
    }
}