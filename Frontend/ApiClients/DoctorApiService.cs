using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend.Models;

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

        // Add a new doctor
        public async Task<bool> AddDoctorAsync(DoctorJointModel doctor)
        {
            Debug.WriteLine($"Adding doctor with UserId: {doctor.UserId}, DepartmentId: {doctor.DepartmentId}, LicenseNumber: {doctor.LicenseNumber}");
            //Debug.WriteLine($"Doctor user: {doctor.User.Name}");
            var response = await _httpClient.PostAsJsonAsync("Doctors", doctor);
            Debug.WriteLine($"Response status code: {response.StatusCode}");
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Error message: {errorMessage}");
            }
            else
            {
                Debug.WriteLine($"Doctor added successfully");
            }
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
        public async Task<bool> IsUserAlreadyDoctorAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"Doctors/is-user-already-doctor/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a user exists in the system
        public async Task<bool> DoesUserExistAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"Doctors/does-user-exist/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a user is a doctor
        public async Task<bool> IsUserDoctorAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"Doctors/check-doctor/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a department exists
        public async Task<bool> DoesDepartmentExistAsync(int departmentId)
        {
            var response = await _httpClient.GetAsync($"Doctors/does-department-exist/{departmentId}");
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadFromJsonAsync<bool>();
            Debug.WriteLine($"Department ID: {departmentId}, Exists: {res}");
            return res;
        }

        // Check if a user exists in the doctors table
        public async Task<bool> UserExistsInDoctorsAsync(int userId, int doctorId)
        {
            var response = await _httpClient.GetAsync($"Doctors/user-exists-in-doctors/{userId}/{doctorId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a doctor exists
        public async Task<bool> DoesDoctorExistAsync(int doctorId)
        {
            var response = await _httpClient.GetAsync($"Doctors/doctor-exists/{doctorId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
}