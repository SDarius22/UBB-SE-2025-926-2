using Hospital.DatabaseServices.Interfaces;
using Hospital.Managers;
using Hospital.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hospital.ViewModels
{
    public class MedicalRecordCreationFormViewModel : INotifyPropertyChanged
    {
        // Dependencies
        private readonly IDoctorManager _doctorManager;
        private readonly IMedicalProcedureManager _procedureManager;

        // Lists
        public ObservableCollection<DoctorJointModel> DoctorsList { get; set; } = new();
        public ObservableCollection<DepartmentModel> DepartmentsList { get; set; } = new();
        public ObservableCollection<PatientJointModel> PatientsList { get; set; } = new();
        public ObservableCollection<ProcedureModel> ProceduresList { get; set; } = new();

        private int _patientId;
        public int PatientId
        {
            get => _patientId;
            set { _patientId = value; OnPropertyChanged(nameof(PatientId)); }
        }

        // Selections
        private DoctorJointModel _selectedDoctor;
        public DoctorJointModel SelectedDoctor
        {
            get => _selectedDoctor;
            set { _selectedDoctor = value; OnPropertyChanged(nameof(SelectedDoctor)); }
        }

        private PatientJointModel _selectedPatient;
        public PatientJointModel SelectedPatient
        {
            get => _selectedPatient;
            set { _selectedPatient = value; OnPropertyChanged(nameof(SelectedPatient)); }
        }

        private ProcedureModel _selectedProcedure;
        public ProcedureModel SelectedProcedure
        {
            get => _selectedProcedure;
            set { _selectedProcedure = value; OnPropertyChanged(nameof(SelectedProcedure)); }
        }

        private DepartmentModel _selectedDepartment;
        public DepartmentModel SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged(nameof(SelectedDepartment));

                // Trigger loading of dependent data
                _ = LoadDoctorsAndProceduresAsync(_selectedDepartment?.DepartmentID ?? 0);
            }
        }

        // Inputs
        private DateTimeOffset _appointmentDateOffset = DateTimeOffset.Now;
        public DateTimeOffset AppointmentDateOffset
        {
            get => _appointmentDateOffset;
            set { _appointmentDateOffset = value; OnPropertyChanged(nameof(AppointmentDateOffset)); }
        }

        private string _appointmentTime;
        public string AppointmentTime
        {
            get => _appointmentTime;
            set { _appointmentTime = value; OnPropertyChanged(nameof(AppointmentTime)); }
        }

        private string _conclusion;
        public string Conclusion
        {
            get => _conclusion;
            set { _conclusion = value; OnPropertyChanged(nameof(Conclusion)); }
        }

        public ObservableCollection<string> DocumentPaths { get; set; } = new();

        public void AddDocument(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
                DocumentPaths.Add(path);
        }

        public DateTime AppointmentDate
        {
            get
            {
                if (TimeSpan.TryParse(AppointmentTime, out var time))
                    return AppointmentDateOffset.Date + time;

                return AppointmentDateOffset.Date;
            }
        }

        // Load doctors and procedures by department
        private async Task LoadDoctorsAndProceduresAsync(int departmentId)
        {
            DoctorsList.Clear();
            ProceduresList.Clear();

            if (departmentId == 0) return;

            await _doctorManager.LoadDoctors(departmentId);
            foreach (var doctor in _doctorManager.GetDoctorsWithRatings())
                DoctorsList.Add(doctor);

            await _procedureManager.LoadProceduresByDepartmentId(departmentId);
            foreach (var proc in _procedureManager.GetProcedures())
                ProceduresList.Add(proc);

            SelectedDoctor = null;
            SelectedProcedure = null;

            Debug.WriteLine($"Loaded {DoctorsList.Count} doctors and {ProceduresList.Count} procedures.");
        }

        // Constructor
        private readonly IMedicalRecordsDatabaseService _medicalRecordsDbService;

        public MedicalRecordCreationFormViewModel(IDoctorManager doctorManager, IMedicalProcedureManager procedureManager, IMedicalRecordsDatabaseService medicalRecordsDbService)
        {
            _doctorManager = doctorManager;
            _procedureManager = procedureManager;
            _medicalRecordsDbService = medicalRecordsDbService;
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}