using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend.ApiClients.Interface;
using Frontend.Models;

namespace Hospital.ApiClients
{
    public class RatingApiService : IRatingApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public RatingApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<RatingModel> GetRatingByMedicalRecordAsync(int medicalRecordId)
        {
            var response = await _httpClient.GetAsync($"Rating/medicalrecord/{medicalRecordId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<RatingModel>();
        }
        public async Task<bool> AddRatingAsync(RatingModel rating)
        {
            var response = await _httpClient.PostAsJsonAsync("Rating", rating);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> RemoveRatingAsync(int ratingId)
        {
            var response = await _httpClient.DeleteAsync($"Rating/{ratingId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
}