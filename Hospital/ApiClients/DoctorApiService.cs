using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.ApiClients
{
    public class DoctorApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public DoctorApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        // Get all doctors
        public async Task<List<DoctorJointModel>> GetDoctorsAsync()
        {
            var response = await _httpClient.GetAsync("Doctors");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DoctorJointModel>>();
        }

        // Get doctors by department
        public async Task<List<DoctorJointModel>> GetDoctorsByDepartmentAsync(int departmentId)
        {
            var response = await _httpClient.GetAsync($"Doctors/department/{departmentId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DoctorJointModel>>();
        }

        // Get shifts for a specific doctor
        public async Task<List<ShiftModel>> GetShiftsForDoctorAsync(int doctorId)
        {
            var response = await _httpClient.GetAsync($"Doctors/{doctorId}/shifts");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ShiftModel>>();
        }

        // Get salary for a specific doctor
        public async Task<double> GetDoctorSalaryAsync(int doctorId)
        {
            var response = await _httpClient.GetAsync($"Doctors/{doctorId}/salary");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<double>();
        }

        // Add a new doctor
        public async Task<bool> AddDoctorAsync(DoctorJointModel doctor)
        {
            var response = await _httpClient.PostAsJsonAsync("Doctors", doctor);
            return response.IsSuccessStatusCode;
        }

        // Update an existing doctor
        public async Task<bool> UpdateDoctorAsync(int doctorId, DoctorJointModel doctor)
        {
            var response = await _httpClient.PutAsJsonAsync($"Doctors/{doctorId}", doctor);
            return response.IsSuccessStatusCode;
        }

        // Delete a doctor
        public async Task<bool> DeleteDoctorAsync(int doctorId)
        {
            var response = await _httpClient.DeleteAsync($"Doctors/{doctorId}");
            return response.IsSuccessStatusCode;
        }

        // Check if a user is already a doctor
        public async Task<bool> IsUserDoctorAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"Doctors/check-doctor/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
}
