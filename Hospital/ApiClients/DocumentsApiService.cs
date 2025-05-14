using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.ApiClients
{
    public class DocumentsApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public DocumentsApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        // Helper method to create an HttpRequestMessage with the Authorization header
        private HttpRequestMessage CreateRequest(HttpMethod method, string url)
        {
            if (string.IsNullOrEmpty(App.Token))
                throw new InvalidOperationException("JWT token is missing. Please log in first.");

            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.Token);
            return request;
        }

        // Upload a document
        public async Task<bool> UploadDocumentAsync(DocumentModel document)
        {
            var request = CreateRequest(HttpMethod.Post, "Documents");
            request.Content = JsonContent.Create(document);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Get documents by medical record ID
        public async Task<List<DocumentModel>> GetDocumentsByMedicalRecordIdAsync(int medicalRecordId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Documents/medicalRecord/{medicalRecordId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DocumentModel>>();
        }
    }
}