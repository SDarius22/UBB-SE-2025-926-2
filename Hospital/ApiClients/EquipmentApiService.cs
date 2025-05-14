using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.ApiClients
{
    public class EquipmentApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public EquipmentApiService()
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

        // Get all equipment
        public async Task<List<EquipmentModel>> GetEquipmentsAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "Equipment");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<EquipmentModel>>();
        }

        // Add a new equipment
        public async Task<bool> AddEquipmentAsync(EquipmentModel equipment)
        {
            var request = CreateRequest(HttpMethod.Post, "Equipment");
            request.Content = JsonContent.Create(equipment);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Update an existing equipment
        public async Task<bool> UpdateEquipmentAsync(int equipmentId, EquipmentModel equipment)
        {
            var request = CreateRequest(HttpMethod.Put, $"Equipment/{equipmentId}");
            request.Content = JsonContent.Create(equipment);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Delete equipment
        public async Task<bool> DeleteEquipmentAsync(int equipmentId)
        {
            var request = CreateRequest(HttpMethod.Delete, $"Equipment/{equipmentId}");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Check if equipment exists
        public async Task<bool> DoesEquipmentExistAsync(int equipmentId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Equipment/exists/{equipmentId}");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                // Assuming that the response content will be a boolean value indicating existence
                var exists = await response.Content.ReadFromJsonAsync<bool>();
                return exists;
            }
            return false;
        }
    }
}