using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend.ApiClients.Interface;
using Frontend.Models;

namespace Hospital.ApiClients
{
    public class DocumentsApiService : IDocumentsApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public DocumentsApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        // Upload a document
        public async Task<bool> UploadDocumentAsync(DocumentModel document)
        {
            var response = await _httpClient.PostAsJsonAsync("Documents", document);
            return response.IsSuccessStatusCode;
        }

        // Get documents by medical record ID
        public async Task<List<DocumentModel>> GetDocumentsByMedicalRecordIdAsync(int medicalRecordId)
        {
            var response = await _httpClient.GetAsync($"Documents/medicalRecord/{medicalRecordId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DocumentModel>>();
        }
    }
}