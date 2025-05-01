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

        public async Task<int> AddMedicalRecordAsync(MedicalRecordModel medicalRecord)
        {
            var response = await _httpClient.PostAsJsonAsync("MedicalRecords", medicalRecord);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<List<MedicalRecordJointModel>> GetMedicalRecordsForPatientAsync(int patientId)
        {
            var response = await _httpClient.GetAsync($"MedicalRecords/patient/{patientId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<MedicalRecordJointModel>>();
        }

        public async Task<MedicalRecordJointModel> GetMedicalRecordByIdAsync(int medicalRecordId)
        {
            var response = await _httpClient.GetAsync($"MedicalRecords/{medicalRecordId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MedicalRecordJointModel>();
        }

        public async Task<List<MedicalRecordJointModel>> GetMedicalRecordsForDoctorAsync(int doctorId)
        {
            var response = await _httpClient.GetAsync($"MedicalRecords/doctor/{doctorId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<MedicalRecordJointModel>>();
        }
    }
}