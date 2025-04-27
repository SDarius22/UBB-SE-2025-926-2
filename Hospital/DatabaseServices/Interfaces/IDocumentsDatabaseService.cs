using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public interface IDocumentDatabaseService
    {
        Task<bool> UploadDocumentToDataBase(DocumentModel document);
        Task<List<DocumentModel>> GetDocumentsByMedicalRecordId(int medicalRecordId);
    }
}
