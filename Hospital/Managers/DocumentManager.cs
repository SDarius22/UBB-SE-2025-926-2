using Hospital.DatabaseServices;
using Hospital.Exceptions;
using Hospital.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Threading.Tasks;
using DocumentModel = Hospital.Models.DocumentModel;
using System.Linq;
using System.Text;

namespace Hospital.Managers
{
    public class DocumentManager : IDocumentManager
    {
        private readonly IDocumentDatabaseService _documentDatabaseService;
        private readonly IFileService _fileService;
        private List<DocumentModel> _documents;

        public DocumentManager(IDocumentDatabaseService documentDatabaseService, IFileService fileService)
        {
            _documentDatabaseService = documentDatabaseService;
            _fileService = fileService;
            _documents = new List<DocumentModel>();
        }

        public async Task LoadDocuments(int medicalRecordId)
        {
            _documents = await _documentDatabaseService.GetDocumentsByMedicalRecordId(medicalRecordId);
        }

        public List<DocumentModel> GetDocuments()
        {
            return _documents;
        }

        public bool HasDocuments(int medicalRecordId)
        {
            return _documents.Any(d => d.MedicalRecordId == medicalRecordId);
        }

        public async Task AddDocumentToMedicalRecord(DocumentModel document)
        {
            try
            {
                bool success = await _documentDatabaseService.UploadDocumentToDataBase(document).ConfigureAwait(false);
                if (success)
                {
                    _documents.Add(document);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error adding document: {exception.Message}");
            }
        }

        public async Task DownloadDocuments(int patientId)
        {
            if (!_documents.Any())
            {
                throw new DocumentNotFoundException("No documents available for download.");
            }

            var filePaths = _documents.Select(d => d.Files).ToList();
            var zipFilePath = await _fileService.CreateAndSaveZipFile(filePaths);
            Process.Start("explorer.exe", $"/select, {zipFilePath}");
        }
    }
}
