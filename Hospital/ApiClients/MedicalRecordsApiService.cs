using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.ApiClients
{
    public class MedicalRecordsApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public MedicalRecordsApiService()
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

        // Add a new medical record
        public async Task<int> AddMedicalRecordAsync(MedicalRecordModel medicalRecord)
        {
            var request = CreateRequest(HttpMethod.Post, "MedicalRecords");
            request.Content = JsonContent.Create(medicalRecord);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        // Get medical records for a patient
        public async Task<List<MedicalRecordJointModel>> GetMedicalRecordsForPatientAsync(int patientId)
        {
            var request = CreateRequest(HttpMethod.Get, $"MedicalRecords/patient/{patientId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<MedicalRecordJointModel>>();
        }

        // Get medical record by ID
        public async Task<MedicalRecordJointModel> GetMedicalRecordByIdAsync(int medicalRecordId)
        {
            var request = CreateRequest(HttpMethod.Get, $"MedicalRecords/{medicalRecordId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MedicalRecordJointModel>();
        }

        // Get medical records for a doctor
        public async Task<List<MedicalRecordJointModel>> GetMedicalRecordsForDoctorAsync(int doctorId)
        {
            var request = CreateRequest(HttpMethod.Get, $"MedicalRecords/doctor/{doctorId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<MedicalRecordJointModel>>();
        }
    }
}