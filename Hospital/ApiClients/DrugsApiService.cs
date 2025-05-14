using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.ApiClients
{
    public class DrugsApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public DrugsApiService()
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

        // Get all drugs
        public async Task<List<DrugModel>> GetDrugsAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "Drugs");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DrugModel>>();
        }

        // Add a new drug
        public async Task<bool> AddDrugAsync(DrugModel drug)
        {
            var request = CreateRequest(HttpMethod.Post, "Drugs");
            request.Content = JsonContent.Create(drug);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Update an existing drug
        public async Task<bool> UpdateDrugAsync(int drugId, DrugModel drug)
        {
            var request = CreateRequest(HttpMethod.Put, $"Drugs/{drugId}");
            request.Content = JsonContent.Create(drug);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Delete a drug
        public async Task<bool> DeleteDrugAsync(int drugId)
        {
            var request = CreateRequest(HttpMethod.Delete, $"Drugs/{drugId}");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Check if a drug exists
        public async Task<bool> DoesDrugExistAsync(int drugId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Drugs/{drugId}");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode; // If it exists, it returns a 200 OK
        }
    }
}