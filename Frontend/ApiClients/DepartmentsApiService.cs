using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend.ApiClients.Interface;
using Frontend.Models;

namespace Hospital.ApiClients
{
    public class DepartmentsApiService : IDepartmentsApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public DepartmentsApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        // Get all departments
        public async Task<List<DepartmentModel>> GetDepartmentsAsync()
        {
            var response = await _httpClient.GetAsync("Departments");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DepartmentModel>>();
        }

        public async Task<DepartmentModel> GetDepartmentAsync(int id)
        {
            var response = await _httpClient.GetAsync("Departments");
            response.EnsureSuccessStatusCode();
            
            var departments = await response.Content.ReadFromJsonAsync<List<DepartmentModel>>();

            return departments.FirstOrDefault(d => d.DepartmentID == id);
        }

        // Add a new department
        public async Task<bool> AddDepartmentAsync(DepartmentModel department)
        {
            var response = await _httpClient.PostAsJsonAsync("Departments", department);
            return response.IsSuccessStatusCode;
        }

        // Update an existing department
        public async Task<bool> UpdateDepartmentAsync(int departmentId, DepartmentModel department)
        {
            var response = await _httpClient.PutAsJsonAsync($"Departments/{departmentId}", department);
            return response.IsSuccessStatusCode;
        }

        // Delete a department
        public async Task<bool> DeleteDepartmentAsync(int departmentId)
        {
            var response = await _httpClient.DeleteAsync($"Departments/{departmentId}");
            return response.IsSuccessStatusCode;
        }

        // Check if a department exists
        public async Task<bool> DoesDepartmentExistAsync(int departmentId)
        {
            var response = await _httpClient.GetAsync($"Departments/exists/{departmentId}");
            if (response.IsSuccessStatusCode)
            {
                var exists = await response.Content.ReadFromJsonAsync<bool>();
                return exists;
            }
            return false;
        }
    }
}