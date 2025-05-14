using Frontend.Models;

namespace Frontend.ApiClients.Interface;

public interface IDocumentsApiService
{
    // Upload a document
    public Task<bool> UploadDocumentAsync(DocumentModel document);

    // Get documents by medical record ID
    public Task<List<DocumentModel>> GetDocumentsByMedicalRecordIdAsync(int medicalRecordId);
}