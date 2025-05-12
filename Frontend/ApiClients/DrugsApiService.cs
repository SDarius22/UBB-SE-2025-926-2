using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend.Models;

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

        // Get all drugs
        public async Task<List<DrugModel>> GetDrugsAsync()
        {
            var response = await _httpClient.GetAsync("Drugs");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DrugModel>>();
        }

        // Add a new drug
        public async Task<bool> AddDrugAsync(DrugModel drug)
        {
            var response = await _httpClient.PostAsJsonAsync("Drugs", drug);
            return response.IsSuccessStatusCode;
        }


        // Update an existing drug
        public async Task<bool> UpdateDrugAsync(int drugId, DrugModel drug)
        {
            var response = await _httpClient.PutAsJsonAsync($"Drugs/{drugId}", drug);
            return response.IsSuccessStatusCode;
        }

        // Delete a drug
        public async Task<bool> DeleteDrugAsync(int drugId)
        {
            var response = await _httpClient.DeleteAsync($"Drugs/{drugId}");
            return response.IsSuccessStatusCode;
        }

        // Check if a drug exists
        public async Task<bool> DoesDrugExistAsync(int drugId)
        {
            var response = await _httpClient.GetAsync($"Drugs/{drugId}");
            return response.IsSuccessStatusCode; // If it exists, it returns a 200 OK
        }
    }
}