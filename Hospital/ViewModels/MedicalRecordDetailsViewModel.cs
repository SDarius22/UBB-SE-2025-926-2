using Hospital.Commands;
using Hospital.Exceptions;
using Hospital.Managers;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hospital.ViewModels
{
    class MedicalRecordDetailsViewModel
    {
        private readonly IDocumentManager _documentManager;
        public MedicalRecordJointModel MedicalRecord { get; private set; }
        public ObservableCollection<DocumentModel> Documents { get; private set; }

        public MedicalRecordDetailsViewModel(MedicalRecordJointModel medicalRecord, IDocumentManager documentManager)
        {
            MedicalRecord = medicalRecord;
            _documentManager = documentManager;
            _documentManager.LoadDocuments(MedicalRecord.MedicalRecordId);
            Documents = new ObservableCollection<DocumentModel>(_documentManager.GetDocuments());
        }

        public async Task OnDownloadButtonClicked()
        {
            await _documentManager.DownloadDocuments(MedicalRecord.PatientId);
        }

        public bool getDownloadButtonIsEnabled()
        {
            return _documentManager.HasDocuments(MedicalRecord.MedicalRecordId);
        }
    }
}
