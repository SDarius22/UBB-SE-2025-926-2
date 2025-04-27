using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Hospital.Managers;
using Hospital.Models;
using System.Linq;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System.Collections.Generic;

namespace Hospital.ViewModels
{
    public class PatientScheduleViewModel
    {
        private readonly IAppointmentManager _appointmentManager;
        public ObservableCollection<TimeSlotModel> DailyAppointments { get; private set; }
        public ObservableCollection<DateTimeOffset> HighlightedDates { get; private set; }

        public DateTimeOffset MinDate { get; private set; }
        public DateTimeOffset MaxDate { get; private set; }

        public PatientScheduleViewModel(IAppointmentManager appointmentManager)
        {
            _appointmentManager = appointmentManager;
            DailyAppointments = new ObservableCollection<TimeSlotModel>();
            HighlightedDates = new ObservableCollection<DateTimeOffset>();
            InitializeDateRange();
        }

        private void InitializeDateRange()
        {
            DateTime now = DateTime.Now;
            MinDate = new DateTime(now.Year, now.Month, 1);
            MaxDate = MinDate.AddMonths(1).AddDays(-1);

        }

        public async Task LoadAppointmentsForPatient(int patientId)
        {
            await _appointmentManager.LoadAppointmentsForPatient(patientId);
            UpdateHighlightedDates();
        }

        private void UpdateHighlightedDates()
        {
            HighlightedDates.Clear();
            foreach (var appointment in _appointmentManager.Appointments)
            {
                HighlightedDates.Add(new DateTimeOffset(appointment.DateAndTime.Date));
            }
        }

        public void UpdateDailySchedule(DateTime selectedDate)
        {
            DailyAppointments.Clear();
            var timeSlots = GenerateTimeSlots(selectedDate);

            var selectedAppointments = _appointmentManager.Appointments
                .Where(a => a.DateAndTime.Date == selectedDate)
                .OrderBy(a => a.DateAndTime.TimeOfDay)
                .ToList();

            foreach (var appointment in selectedAppointments)
            {
                DateTime appointmentStart = appointment.DateAndTime;
                DateTime appointmentEnd = appointmentStart.Add(appointment.ProcedureDuration);

                foreach (var slot in timeSlots)
                {
                    if (slot.TimeSlot >= appointmentStart && slot.TimeSlot < appointmentEnd)
                    {
                        slot.Appointment = appointment.ProcedureName;
                        slot.HighlightStatus = "Available";
                    }
                }
            }

            foreach (var slot in timeSlots)
            {
                DailyAppointments.Add(slot);
            }
        }

        private List<TimeSlotModel> GenerateTimeSlots(DateTime date)
        {
            List<TimeSlotModel> slots = new List<TimeSlotModel>();
            DateTime startTime = date.Date.AddHours(8); // Start at 8:00 AM
            DateTime endTime = date.Date.AddHours(18);  // End at 6:00 PM

            while (startTime < endTime)
            {
                slots.Add(new TimeSlotModel
                {
                    TimeSlot = startTime,
                    Time = startTime.ToString("hh:mm tt"),
                    Appointment = "",
                    HighlightStatus = "None"
                });

                startTime = startTime.AddMinutes(30);
            }

            return slots;
        }

        public AppointmentJointModel GetAppointmentForTimeSlot(TimeSlotModel selectedSlot, DateTime selectedDate)
        {
            if (string.IsNullOrEmpty(selectedSlot?.Appointment))
                return null;

            return _appointmentManager.Appointments
                .FirstOrDefault(a =>
                    a.ProcedureName == selectedSlot.Appointment &&
                    a.DateAndTime.Date == selectedDate);
        }

        public bool CanCancelAppointment(AppointmentJointModel appointment)
        {
            return (appointment.DateAndTime.ToLocalTime() - DateTime.Now).TotalHours >= 24;
        }

        public async Task CancelAppointment(AppointmentJointModel appointment)
        {
            if (!CanCancelAppointment(appointment))
                throw new InvalidOperationException("Appointments can only be cancelled more than 24 hours in advance.");

            await _appointmentManager.RemoveAppointment(appointment.AppointmentId);
        }

        public bool HasAppointmentsOnDate(DateTime selectedDate)
        {
            return _appointmentManager.Appointments.Any(a => a.DateAndTime.Date == selectedDate);
        }
    }
}
