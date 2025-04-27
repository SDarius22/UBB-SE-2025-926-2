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

        public ShiftModel GetShiftByDay(DateTime day)
        {
            ShiftModel? shiftByDate = _shifts.FirstOrDefault(shift => shift.DateTime.Date == day.Date);
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

        public List<TimeSlotModel> GenerateTimeSlots(DateTime date, List<ShiftModel> shifts, List<AppointmentJointModel> appointments)
        {
            List<TimeSlotModel> slots = new();
            DateTime startTime = date.Date;
            DateTime endTime = startTime.AddDays(1);
            const string TimeFormat = "hh:mm tt";
            const int TimeSlotIntervalInMinutes = 30;

            var selectedAppointments = appointments
                .Where(a => a.DateAndTime.Date == date.Date)
                .ToList();

            var selectedShifts = shifts
                .Where(shift =>
                {
                    var shiftStart = shift.DateTime.Date + shift.StartTime;
                    var shiftEnd = shift.DateTime.Date + shift.EndTime;

                    if (shift.EndTime <= shift.StartTime)
                        shiftEnd = shiftEnd.AddDays(1);

                    return shiftStart < endTime && shiftEnd > startTime;
                })
                .ToList();

            while (startTime < endTime)
            {
                var slot = new TimeSlotModel
                {
                    TimeSlot = startTime,
                    Time = startTime.ToString(TimeFormat),
                    Appointment = "",
                    HighlightStatus = "None"
                };

                String highlightStatus = "None";

                bool isInShift = selectedShifts.Any(shift =>
                {
                    DateTime shiftStart = shift.DateTime.Date + shift.StartTime;
                    DateTime shiftEnd = shift.DateTime.Date + shift.EndTime;
                    if (shift.EndTime <= shift.StartTime)
                        shiftEnd = shiftEnd.AddDays(1);

                    return startTime >= shiftStart && startTime < shiftEnd;
                });

                var matchingAppointment = selectedAppointments.FirstOrDefault(appointment =>
                    appointment.DateAndTime == startTime && isInShift);

                if (matchingAppointment != null)
                {
                    slot.Appointment = matchingAppointment.ProcedureName;
                    highlightStatus = "Booked";
                }
                else if (isInShift)
                {
                    highlightStatus = "Available";
                }

                slot.HighlightStatus = highlightStatus;
                slots.Add(slot);
                startTime = startTime.AddMinutes(TimeSlotIntervalInMinutes);
            }

            return slots;
        }
    }
}
