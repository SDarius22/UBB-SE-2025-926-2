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

        // Helper method to create an HttpRequestMessage with the Authorization header
        private HttpRequestMessage CreateRequest(HttpMethod method, string url)
        {
            if (string.IsNullOrEmpty(App.Token))
                throw new InvalidOperationException("JWT token is missing. Please log in first.");

            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.Token);
            return request;
        }

        // Get all departments
        public async Task<List<DepartmentModel>> GetDepartmentsAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "Departments");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DepartmentModel>>();
        }

        // Add a new department
        public async Task<bool> AddDepartmentAsync(DepartmentModel department)
        {
            var request = CreateRequest(HttpMethod.Post, "Departments");
            request.Content = JsonContent.Create(department);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Update an existing department
        public async Task<bool> UpdateDepartmentAsync(int departmentId, DepartmentModel department)
        {
            var request = CreateRequest(HttpMethod.Put, $"Departments/{departmentId}");
            request.Content = JsonContent.Create(department);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Delete a department
        public async Task<bool> DeleteDepartmentAsync(int departmentId)
        {
            var request = CreateRequest(HttpMethod.Delete, $"Departments/{departmentId}");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Check if a department exists
        public async Task<bool> DoesDepartmentExistAsync(int departmentId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Departments/exists/{departmentId}");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            return false;
        }
    }
}