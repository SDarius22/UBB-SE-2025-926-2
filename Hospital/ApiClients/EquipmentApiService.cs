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

        // Get all equipment
        public async Task<List<EquipmentModel>> GetEquipmentsAsync()
        {
            var response = await _httpClient.GetAsync("Equipment");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<EquipmentModel>>();
        }

        // Add a new equipment
        public async Task<bool> AddEquipmentAsync(EquipmentModel equipment)
        {
            var response = await _httpClient.PostAsJsonAsync("Equipment", equipment);
            return response.IsSuccessStatusCode;
        }

        // Update an existing equipment
        public async Task<bool> UpdateEquipmentAsync(int equipmentId, EquipmentModel equipment)
        {
            var response = await _httpClient.PutAsJsonAsync($"Equipment/{equipmentId}", equipment);
            return response.IsSuccessStatusCode;
        }

        // Delete equipment
        public async Task<bool> DeleteEquipmentAsync(int equipmentId)
        {
            var response = await _httpClient.DeleteAsync($"Equipment/{equipmentId}");
            return response.IsSuccessStatusCode;
        }

        // Check if equipment exists
        public async Task<bool> DoesEquipmentExistAsync(int equipmentId)
        {
            var response = await _httpClient.GetAsync($"Equipment/exists/{equipmentId}");
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