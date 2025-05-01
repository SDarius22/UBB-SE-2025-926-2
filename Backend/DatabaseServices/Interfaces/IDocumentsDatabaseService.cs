using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.DatabaseServices
{
    public interface IDocumentDatabaseService
    {
        Task<bool> UploadDocumentToDataBase(DocumentModel document);
        Task<List<DocumentModel>> GetDocumentsByMedicalRecordId(int medicalRecordId);
    }
}
