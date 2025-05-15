using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend.ApiClients.Interface;
using Frontend.Models;

namespace Hospital.ApiClients
{
    public class EquipmentApiService : IEquipmentApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public EquipmentApiService(IHttpContextAccessor accessor)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            var token = accessor.HttpContext?.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Get all equipment
        public async Task<List<EquipmentModel>> GetEquipmentsAsync()
        {
            var response = await _httpClient.GetAsync("Equipment");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<EquipmentModel>>();
        }

        public async Task<EquipmentModel> GetEquipmentAsync(int id)
        {
            var response = await _httpClient.GetAsync("Equipment");
            response.EnsureSuccessStatusCode();

            var equipments = await response.Content.ReadFromJsonAsync<List<EquipmentModel>>();

            return equipments.FirstOrDefault(d => d.EquipmentID == id);
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