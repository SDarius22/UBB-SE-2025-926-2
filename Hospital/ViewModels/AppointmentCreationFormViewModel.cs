using Hospital.Commands;
using Hospital.Configs;
using Hospital.DatabaseServices;
using Hospital.Exceptions;
using Hospital.Helpers;
using Hospital.Managers;
using Hospital.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.Foundation;
using Windows.UI.Popups;

namespace Hospital.ViewModels
{
    public class AppointmentCreationFormViewModel : INotifyPropertyChanged
    {


        // Configuration or constants
        private const int DefaultAppointmentId = 0;

        private const int DefaultShiftHours = 12;

        private const int MaxAppointmentBookingRangeInMonths = 1;

        private const bool DefaultAppointmentIsFinished = false;

        // List Properties

        public ObservableCollection<DepartmentModel>? DepartmentsList { get; set; }
        public ObservableCollection<ProcedureModel>? ProceduresList { get; set; }

        public ObservableCollection<DoctorJointModel>? DoctorsList { get; set; }

        private List<ShiftModel>? _shiftsList { get; set; }

        private List<AppointmentJointModel>? AppointmentsList { get; set; }

        public ObservableCollection<string> HoursList { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<DateTimeOffset> HighlightedDates { get; set; } = new ObservableCollection<DateTimeOffset>();

        // Calendar Dates
        public DateTimeOffset MinimumDate { get; set; }

        public DateTimeOffset MaximumDate { get; set; }




        //Manager Models
        private IDepartmentManager _departmentManager;
        private IMedicalProcedureManager _procedureManager;
        private IDoctorManager _doctorManager;
        private IShiftManager _shiftManager;
        private IAppointmentManager _appointmentManager;

        //public event
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //selected fields
        public DepartmentModel? SelectedDepartment { get; set; }
        public ProcedureModel? SelectedProcedure { get; set; }
        public DoctorJointModel? SelectedDoctor { get; set; }
        public TimeSpan? SelectedTime { get; set; }

        private DateTimeOffset? _selectedCalendarDate = null;
        public DateTimeOffset? SelectedCalendarDate
        {
            get => _selectedCalendarDate;
            set
            {
                _selectedCalendarDate = value;
                OnPropertyChanged(nameof(SelectedCalendarDate));
                Task availableTimeSlotsTask = LoadAvailableTimeSlots();
            }
        }

        private string _selectedHour;
        public string SelectedHour
        {
            get => _selectedHour;
            set
            {
                _selectedHour = value;
                OnPropertyChanged(nameof(SelectedHour));
                // Optionally update SelectedTime when the hour changes:
                if (!string.IsNullOrWhiteSpace(_selectedHour))
                {
                    // Parse the string to a TimeSpan. Ensure the format matches (HH:mm)
                    SelectedTime = TimeSpan.Parse(_selectedHour);
                }
            }
        }

        // Feedback to the UI
        private string _bookingStatusMessage;
        public string BookingStatusMessage
        {
            get => _bookingStatusMessage;
            set
            {
                _bookingStatusMessage = value;
                OnPropertyChanged(nameof(BookingStatusMessage));
            }
        }

        //disable controls
        private bool _areProceduresAndDoctorsEnabled = true;
        public bool AreProceduresAndDoctorsEnabled
        {
            get => _areProceduresAndDoctorsEnabled;
            set
            {
                _areProceduresAndDoctorsEnabled = value;
                OnPropertyChanged(nameof(AreProceduresAndDoctorsEnabled));
            }
        }


        private bool _isDateEnabled = true;
        public bool IsDateEnabled
        {
            get => _isDateEnabled;
            set
            {
                _isDateEnabled = value;
                OnPropertyChanged(nameof(IsDateEnabled));
            }
        }


        private bool _isTimeEnabled = true;
        public bool IsTimeEnabled
        {
            get => _isTimeEnabled;
            set
            {
                _isTimeEnabled = value;
                Debug.WriteLine("IsTimeEnabled: " + _isTimeEnabled);
                OnPropertyChanged(nameof(IsTimeEnabled));
            }
        }

        //XAML Root
        public XamlRoot? Root { get; set; }

        private AppointmentCreationFormViewModel(IDepartmentManager departmentManagerModel, IMedicalProcedureManager procedureManagerModel, IDoctorManager doctorManagerModel, ShiftManager shiftManagerModel, IAppointmentManager appointmentManagerModel)
        {
            _departmentManager = departmentManagerModel;
            _procedureManager = procedureManagerModel;
            _doctorManager = doctorManagerModel;
            _shiftManager = shiftManagerModel;
            _appointmentManager = appointmentManagerModel;

            //initialize lists
            DepartmentsList = new ObservableCollection<DepartmentModel>();
            ProceduresList = new ObservableCollection<ProcedureModel>();
            DoctorsList = new ObservableCollection<DoctorJointModel>();

            //set calendar dates
            MinimumDate = DateTimeOffset.Now;
            MaximumDate = MinimumDate.AddMonths(MaxAppointmentBookingRangeInMonths);
        }

        public static async Task<AppointmentCreationFormViewModel> CreateViewModel(DepartmentManager departmentManagerModel, MedicalProcedureManager procedureManagerModel, DoctorManager doctorManagerModel, ShiftManager shiftManagerModel, Managers.AppointmentManager appointmentManagerModel)
        {
            var appointmentCreationViewModel = new AppointmentCreationFormViewModel(departmentManagerModel, procedureManagerModel, doctorManagerModel, shiftManagerModel, appointmentManagerModel);
            await appointmentCreationViewModel.LoadDepartments();
            return appointmentCreationViewModel;
        }

        private async Task LoadDepartments()
        {
            if (DepartmentsList != null)
                DepartmentsList.Clear();
            await _departmentManager.LoadDepartments();
            foreach (DepartmentModel department in _departmentManager.GetDepartments())
            {
                DepartmentsList?.Add(department);
            }
        }

        public async Task LoadProceduresAndDoctorsOfSelectedDepartment()
        {
            //clear the list
            if (ProceduresList != null)
                ProceduresList.Clear();
            if (DoctorsList != null)
                DoctorsList.Clear();

            //load the procedures
            await _procedureManager.LoadProceduresByDepartmentId(SelectedDepartment.DepartmentId);
            foreach (ProcedureModel procedure in _procedureManager.GetProcedures())
            {
                ProceduresList?.Add(procedure);
            }

            //load the doctors
            await _doctorManager.LoadDoctors(SelectedDepartment.DepartmentId);
            foreach (DoctorJointModel doctor in _doctorManager.GetDoctorsWithRatings())
            {
                DoctorsList?.Add(doctor);
            }

            // Enable controls only if we have both procedures and doctors
            AreProceduresAndDoctorsEnabled = ProceduresList?.Count > 0 && DoctorsList?.Count > 0;
            IsDateEnabled = true;
            IsTimeEnabled = true;
        }

        public async Task LoadDoctorSchedule()
        {
            HighlightedDates.Clear();
            SelectedCalendarDate = null;
            IsDateEnabled = true;
            IsTimeEnabled = true;

            if (SelectedDoctor == null)
            {
                IsDateEnabled = true;
                return;
            }

            await _shiftManager.LoadUpcomingDoctorDayshifts(SelectedDoctor.DoctorId);
            _shiftsList = _shiftManager.GetShifts();

            if (_shiftsList.Count == 0)
            {
                HoursList.Clear();
                IsDateEnabled = true;
                IsTimeEnabled = true;
                return;
            }

            foreach (ShiftModel shift in _shiftsList)
            {
                HighlightedDates.Add(new DateTimeOffset(shift.DateTime));
            }
            IsDateEnabled = true;
        }

        public async Task LoadAvailableTimeSlots()
        {
            //check for all necessary fields
            if (SelectedDoctor == null || SelectedCalendarDate == null || SelectedProcedure == null)
            {
                HoursList.Clear();
                IsTimeEnabled = false;
                return;
            }

            //if there are no shifts return
            if (_shiftsList == null)
            {
                HighlightedDates.Clear();
                HoursList.Clear();
                IsTimeEnabled = false;
                return;
            }

            //get shift for the selected date
            ShiftModel shift;
            try
            {
                shift = _shiftManager.GetShiftByDay(SelectedCalendarDate.Value.Date);
            }
            catch (ShiftNotFoundException exception)
            {
                //if there is no shift for the selected date return
                HoursList.Clear();
                IsTimeEnabled = false;
                return;
            }

            //get appointments for the selected doctor on the selected date
            try
            {
                await _appointmentManager.LoadDoctorAppointmentsOnDate(SelectedDoctor.DoctorId, SelectedCalendarDate.Value.Date);
            }
            catch (Exception exception)
            {
                ContentDialog contentDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = exception.Message,
                    CloseButtonText = "Ok"
                };
                contentDialog.XamlRoot = Root;
                await contentDialog.ShowAsync();
                IsTimeEnabled = false;
                return;
            }

            // get the appointments
            AppointmentsList = _appointmentManager.GetAppointments();

            // compute available time slots
            List<string> availableTimeSlots = new List<string>();

            // get the start time
            TimeSpan startTimeShift = shift.StartTime;

            TimeSpan endTimeShift;
            // handle the 24h shift -- can be changed
            if (shift.StartTime == shift.EndTime)
            {
                endTimeShift = startTimeShift.Add(TimeSpan.FromHours(DefaultShiftHours));
            }
            else
            {
                endTimeShift = shift.EndTime;
            }

            // Round procedure duration to the nearest slot duration multiple
            TimeSpan procedureDuration = TimeRounder.RoundProcedureDuration(SelectedProcedure.ProcedureDuration);

            // generate the time slots
            TimeSpan currentTime = startTimeShift;

            foreach (var appointment in this.AppointmentsList)
            {
                TimeSpan appointmentStartTime = appointment.DateAndTime.TimeOfDay;
                TimeSpan appointmentEndTime = appointmentStartTime.Add(appointment.ProcedureDuration);

                // Round the appointment start time to the nearest 30-minute multiple after the current time
                appointmentEndTime = TimeRounder.RoundProcedureDuration(appointmentEndTime);

                // Check for available slots before the next appointment starts
                while (currentTime + procedureDuration <= appointmentStartTime)
                {
                    availableTimeSlots.Add(currentTime.ToString(@"hh\:mm")); // Format as HH:mm
                    currentTime = currentTime.Add(TimeSpan.FromMinutes(ApplicationConfiguration.GetInstance().SlotDuration));// Move to the next possible slot
                }

                // Move past the current appointment
                currentTime = appointmentEndTime;
            }

            // Check remaining time after the last appointment
            while (currentTime + procedureDuration <= endTimeShift)
            {
                availableTimeSlots.Add(currentTime.ToString(@"hh\:mm"));
                currentTime = currentTime.Add(TimeSpan.FromMinutes(ApplicationConfiguration.GetInstance().SlotDuration));
            }

            // Update the list of available time slots
            this.HoursList.Clear();
            foreach (string timeSlot in availableTimeSlots)
            {
                this.HoursList.Add(timeSlot);
            }

            // Enable time selection only if there are available slots
            IsTimeEnabled = HoursList.Count > 0;
            
            // If there are no available slots, show a message
            if (!IsTimeEnabled)
            {
                ContentDialog noSlotsDialog = new ContentDialog
                {
                    Title = "No Available Slots",
                    Content = "There are no available time slots for the selected date.",
                    CloseButtonText = "Ok"
                };
                noSlotsDialog.XamlRoot = Root;
                await noSlotsDialog.ShowAsync();
            }
        }

        public async Task BookAppointment()
        {
            var date = SelectedCalendarDate.Value.Date; // e.g. 2025-04-01
            var time = TimeSpan.Parse(SelectedHour); // e.g. 14:00:00
            DateTime actualDateTime = date + time; // e.g. 2025-04-01 14:00:00

            //bool appointmentIsFinished = false;

            // Create the new appointment
            var newAppointment = new Models.AppointmentModel(
                DefaultAppointmentId, // Appointment ID (0 so SQL Server auto-generates it)
                SelectedDoctor.DoctorId,
                ApplicationConfiguration.GetInstance().patientId, // Patient ID (adjust as needed)
                actualDateTime,
                DefaultAppointmentIsFinished,   // Finished (initially false)
                SelectedProcedure.ProcedureId
            );


            await _appointmentManager.CreateAppointment(newAppointment);
        }

        // Validate user input
        public bool ValidateAppointment()
        {
            bool isValid = !(SelectedDepartment == null || SelectedProcedure == null || SelectedDoctor == null ||
                SelectedCalendarDate == null || SelectedTime == null);

            return isValid;
        }
    }
}
