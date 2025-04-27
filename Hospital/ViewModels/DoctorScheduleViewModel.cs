using Hospital.Models;
using Hospital.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Managers;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Hospital.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hospital.Views;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Reflection.Metadata;

namespace Hospital.ViewModels
{
    public class DoctorScheduleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Managers
        private readonly IAppointmentManager _appointmentManager;
        private readonly IShiftManager _shiftManager;

        // Observable Collections
        public ObservableCollection<TimeSlotModel> DailySchedule { get; set; } = new();

        public ObservableCollection<DateTimeOffset> ShiftDates { get; set; }

        public List<AppointmentJointModel> Appointments { get; set; }
        public List<ShiftModel> Shifts { get; set; }

        public ICommand OpenDetailsCommand { get; set; }


        private const int TimeSlotIntervalInMinutes = 30; // The time interval for each slot (in minutes)

        private const int DefaultDoctorId = 1; // Default ID for testing
        public int DoctorId { get; set; } = DefaultDoctorId;


        private DateTimeOffset _minimumDateForSelectingAppointment;
        public DateTimeOffset MinimumDateForSelectingAppointment
        {
            get => _minimumDateForSelectingAppointment;
            set
            {
                _minimumDateForSelectingAppointment = value;
                OnPropertyChanged();
            }
        }

        private DateTimeOffset _maximumDateForSelectingAppointment;
        public DateTimeOffset MaximumDateForSelectingAppointment
        {
            get => _maximumDateForSelectingAppointment;
            set
            {
                _maximumDateForSelectingAppointment = value;
                OnPropertyChanged();
            }
        }

        private TimeSlotModel _selectedSlot;
        public TimeSlotModel SelectedSlot
        {
            get => _selectedSlot;
            set
            {
                _selectedSlot = value;
                OnPropertyChanged();
            }
        }

        public DoctorScheduleViewModel(IAppointmentManager appointmentManager, IShiftManager shiftManager)
        {
            _appointmentManager = appointmentManager;
            _shiftManager = shiftManager;
            Appointments = new List<AppointmentJointModel>();
            Shifts = new List<ShiftModel>();
            ShiftDates = new ObservableCollection<DateTimeOffset>();

            var (start, end) = _shiftManager.GetMonthlyCalendarRange();
            MinimumDateForSelectingAppointment = start;
            MaximumDateForSelectingAppointment = end;

            OpenDetailsCommand = new RelayCommand(OpenAppointmentForDoctor);
        }

        private void OpenAppointmentForDoctor(object objectTimeSlot)
        {
            if (objectTimeSlot is not TimeSlotModel selectedSlot) return;

            if (string.IsNullOrEmpty(selectedSlot.Appointment) && selectedSlot.HighlightStatus == "None")
                return;

            SelectedSlot = selectedSlot;
        }



        public async Task LoadAppointmentsForDoctor()
        {
            try
            {
                await _appointmentManager.LoadAppointmentsForDoctor(DoctorId);
                Appointments = _appointmentManager.Appointments;
            }
            catch (Exception exception)
            {
                throw new Exception($"Error loading appointments: {exception.Message}");
            }
        }

        public async Task LoadShiftsForDoctor()
        {
            try
            {
                await _shiftManager.LoadShifts(DoctorId);
                Shifts = _shiftManager.GetShifts();

                ShiftDates.Clear();
                foreach (var shift in Shifts)
                {
                    var shiftStartDate = shift.DateTime.Date;
                    var shiftEndDate = shift.DateTime.Date;

                    if (shift.EndTime <= shift.StartTime)
                    {
                        shiftEndDate = shiftEndDate.AddDays(1);
                    }

                    ShiftDates.Add(new DateTimeOffset(shiftStartDate, TimeSpan.Zero));

                    if (shiftEndDate > shiftStartDate)
                    {
                        ShiftDates.Add(new DateTimeOffset(shiftEndDate, TimeSpan.Zero));
                    }
                }

                OnPropertyChanged(nameof(ShiftDates));
            }
            catch (Exception exception)
            {
                throw new Exception($"Error loading shifts: {exception.Message}");
            }
        }

        public async Task OnDateSelected(DateTime date)
        {
            DailySchedule.Clear();
            try
            {
                await _appointmentManager.LoadDoctorAppointmentsOnDate(DoctorId, date);
                Appointments = _appointmentManager.Appointments;
                await _shiftManager.LoadShifts(DoctorId);
                Shifts = _shiftManager.GetShifts();

                var slots = _shiftManager.GenerateTimeSlots(date, Shifts, Appointments);
                foreach (TimeSlotModel slot in slots) // Explicitly cast or ensure the type is TimeSlotModel
                {
                    this.DailySchedule.Add(slot); // Prefix with 'this' to resolve SA1101
                }
                OnPropertyChanged(nameof(DailySchedule));
            }
            catch (Exception exception)
            {
                throw new Exception($"Database access failed: {exception.Message}");
            }
        }



    }
}
