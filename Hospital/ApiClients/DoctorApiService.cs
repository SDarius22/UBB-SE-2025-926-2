using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
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

        // Helper method to create an HttpRequestMessage with the Authorization header
        private HttpRequestMessage CreateRequest(HttpMethod method, string url)
        {
            if (string.IsNullOrEmpty(App.Token))
                throw new InvalidOperationException("JWT token is missing. Please log in first.");

            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.Token);
            return request;
        }

        // Get all doctors
        public async Task<List<DoctorJointModel>> GetDoctorsAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "Doctors");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DoctorJointModel>>();
        }

        // Get doctors by department
        public async Task<List<DoctorJointModel>> GetDoctorsByDepartmentAsync(int departmentId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Doctors/department/{departmentId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DoctorJointModel>>();
        }

        // Add a new doctor
        public async Task<bool> AddDoctorAsync(DoctorJointModel doctor)
        {
            Debug.WriteLine($"Adding doctor with UserId: {doctor.UserId}, DepartmentId: {doctor.DepartmentId}, LicenseNumber: {doctor.LicenseNumber}");
            var request = CreateRequest(HttpMethod.Post, "Doctors");
            request.Content = JsonContent.Create(doctor);
            var response = await _httpClient.SendAsync(request);

            Debug.WriteLine($"Response status code: {response.StatusCode}");
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Error message: {errorMessage}");
            }
            else if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Doctor added successfully");
            }
            return response.IsSuccessStatusCode;
        }

        // Update an existing doctor
        public async Task<bool> UpdateDoctorAsync(int doctorId, DoctorJointModel doctor)
        {
            var request = CreateRequest(HttpMethod.Put, $"Doctors/{doctorId}");
            request.Content = JsonContent.Create(doctor);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Delete a doctor
        public async Task<bool> DeleteDoctorAsync(int doctorId)
        {
            var request = CreateRequest(HttpMethod.Delete, $"Doctors/{doctorId}");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Check if a user is already a doctor
        public async Task<bool> IsUserAlreadyDoctorAsync(int userId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Doctors/is-user-already-doctor/{userId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a user exists in the system
        public async Task<bool> DoesUserExistAsync(int userId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Doctors/does-user-exist/{userId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a user is a doctor
        public async Task<bool> IsUserDoctorAsync(int userId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Doctors/check-doctor/{userId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a department exists
        public async Task<bool> DoesDepartmentExistAsync(int departmentId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Doctors/does-department-exist/{departmentId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadFromJsonAsync<bool>();
            Debug.WriteLine($"Department ID: {departmentId}, Exists: {res}");
            return res;
        }

        // Check if a user exists in the doctors table
        public async Task<bool> UserExistsInDoctorsAsync(int userId, int doctorId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Doctors/user-exists-in-doctors/{userId}/{doctorId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a doctor exists
        public async Task<bool> DoesDoctorExistAsync(int doctorId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Doctors/doctor-exists/{doctorId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
}