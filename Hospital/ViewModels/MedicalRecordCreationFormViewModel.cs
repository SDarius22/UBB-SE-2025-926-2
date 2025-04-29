using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Managers;

using System.ComponentModel;
using System.Diagnostics;

namespace Hospital.ViewModels
{
    public class MedicalRecordCreationFormViewModel
    {
        private readonly IMedicalRecordManager _medicalRecordManager;
        private readonly IDocumentManager _documentManager;

        private int _patientId;
        private int _doctorId;
        private int _procedureId;
        private string _patientName;
        private string _doctorName;
        private string _appointmentTime;
        private string _department;
        private string _conclusion;
        public ObservableCollection<string> DocumentPaths { get; private set; } = new ObservableCollection<string>();

        public void AddDocument(string path)
        {
            DocumentPaths.Add(path);
        }

        private DateTime _appointmentDate;
        public DateTimeOffset? AppointmentDateOffset
        {
            get
            {
                if (_appointmentDate == DateTime.MinValue)
                    return null;

                return new DateTimeOffset(_appointmentDate);
            }
            set
            {
                if (value.HasValue)
                {
                    _appointmentDate = value.Value.DateTime;
                    OnPropertyChanged(nameof(AppointmentDateOffset));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Documents { get; private set; }

        public MedicalRecordCreationFormViewModel(IMedicalRecordManager medicalRecordManager, IDocumentManager documentManagerModel)
        {
            _medicalRecordManager = medicalRecordManager;
            _documentManager = documentManagerModel;
            _appointmentDate = DateTime.Now;
            Documents = new ObservableCollection<string>();
        }
        
        public MedicalRecordCreationFormViewModel()
        {
            Documents = new ObservableCollection<string>();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //public async Task<int> CreateMedicalRecord(AppointmentJointModel appointment, string conclusion)
        //{
        //    try
        //    {
        //        return await _medicalRecordManager.CreateMedicalRecordWithAppointment(appointment, conclusion);
        //    }
        //    catch (Exception exception)
        //    {
        //        Console.WriteLine($"Error creating medical record: {exception.Message}");
        //        throw;
        //    }
        //}

        public async Task<int> CreateMedicalRecord()
        {
            try
            {
                var medicalRecord = new MedicalRecordModel(
                    medicalRecordId: 0,
                    patientId: this.PatientId,
                    doctorId: this.DoctorId,
                    procedureId: this.ProcedureId,
                    conclusion: this.Conclusion,
                    dateAndTime: this.AppointmentDate
                );

                return await _medicalRecordManager.CreateMedicalRecord(medicalRecord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating medical record: {ex.Message}");
                throw;
            }
        }


        public async Task AddDocument(int medicalRecordId, string path)
        {
            var document = new DocumentModel(0, medicalRecordId, path);
            await _documentManager.AddDocumentToMedicalRecord(document);
            Documents.Add(path);
        }

        public int PatientId
        {
            get => _patientId;
            set { _patientId = value; OnPropertyChanged(nameof(PatientId)); }
        }

        public int DoctorId
        {
            get => _doctorId;
            set { _doctorId = value; OnPropertyChanged(nameof(DoctorId)); }
        }

        public int ProcedureId
        {
            get => _procedureId;
            set { _procedureId = value; OnPropertyChanged(nameof(ProcedureId)); }
        }

        public string PatientName
        {
            get => _patientName;
            set { _patientName = value; OnPropertyChanged(nameof(PatientName)); }
        }

        public string DoctorName
        {
            get => _doctorName;
            set { _doctorName = value; OnPropertyChanged(nameof(DoctorName)); }
        }

        public DateTime AppointmentDate
        {
            get => _appointmentDate;
            set { _appointmentDate = value; OnPropertyChanged(nameof(AppointmentDate)); }
        }

        public string AppointmentTime
        {
            get => _appointmentTime;
            set { _appointmentTime = value; OnPropertyChanged(nameof(AppointmentTime)); }
        }

        public string Department
        {
            get => _department;
            set { _department = value; OnPropertyChanged(nameof(Department)); }
        }

        public string Conclusion
        {
            get => _conclusion;
            set { _conclusion = value; OnPropertyChanged(nameof(Conclusion)); }
        }
    }
}
