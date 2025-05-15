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
    public class DrugsApiService : IDrugsApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public DrugsApiService(IHttpContextAccessor accessor)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            var token = accessor.HttpContext?.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Get all drugs
        public async Task<List<DrugModel>> GetDrugsAsync()
        {
            var response = await _httpClient.GetAsync("Drugs");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DrugModel>>();
        }

        public async Task<DrugModel> GetDrugAsync(int id)
        {
            var response = await _httpClient.GetAsync("Drugs");
            response.EnsureSuccessStatusCode();

            var drugs = await response.Content.ReadFromJsonAsync<List<DrugModel>>();

            return drugs.FirstOrDefault(d => d.DrugID == id);
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