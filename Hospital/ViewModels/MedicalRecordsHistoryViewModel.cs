using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Hospital.Commands;
using Hospital.Managers;
using Hospital.Models;
using Hospital.Views;

namespace Hospital.ViewModels
{
    public class MedicalRecordsHistoryViewModel
    {
        private readonly IMedicalRecordManager _medicalRecordManager;
        private readonly IDocumentManager _documentManager;

        public ObservableCollection<MedicalRecordJointModel> MedicalRecords { get; private set; }

        public MedicalRecordsHistoryViewModel(int patientId, IMedicalRecordManager medicalRecordManager, IDocumentManager documentManager)
        {
            _medicalRecordManager = medicalRecordManager;
            _documentManager = documentManager;
            MedicalRecords = new ObservableCollection<MedicalRecordJointModel>();
            InitializeAsync(patientId);
        }

        private async void InitializeAsync(int patientId)
        {
            try
            {
                await _medicalRecordManager.LoadMedicalRecordsForPatient(patientId);
                var records = await _medicalRecordManager.GetMedicalRecords();
                foreach (var record in records)
                {
                    MedicalRecords.Add(record);
                }
            }
            catch (Exception ex)
            {
                // In a real application, you would want to handle this error appropriately
                // Perhaps raise an event that the View can subscribe to
                Console.WriteLine($"Error loading medical records: {ex.Message}");
            }
        }

        public void ShowMedicalRecordDetails(MedicalRecordJointModel medicalRecord)
        {
            if (medicalRecord == null)
            {
                throw new ArgumentNullException(nameof(medicalRecord));
            }

            var medicalRecordDetailsView = new MedicalRecordDetailsView(medicalRecord, _documentManager);
            medicalRecordDetailsView.Activate();
        }
    }
}
