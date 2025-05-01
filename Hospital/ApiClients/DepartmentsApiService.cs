using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.ApiClients
{
    public class DepartmentsApiService
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
    }
}
