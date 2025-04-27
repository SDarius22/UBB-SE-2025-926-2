using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Managers
{
    public interface IDocumentManager
    {
        List<DocumentModel> GetDocuments();
        Task AddDocumentToMedicalRecord(DocumentModel document);
        Task LoadDocuments(int medicalRecordId);
        Task DownloadDocuments(int patientId);
        bool HasDocuments(int medicalRecordId);
    }
}
