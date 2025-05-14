using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.ApiClients
{
    public class RatingApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public RatingApiService()
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

        // Get rating by medical record ID
        public async Task<RatingModel> GetRatingByMedicalRecordAsync(int medicalRecordId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Rating/medicalrecord/{medicalRecordId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<RatingModel>();
        }

        // Add a new rating
        public async Task<bool> AddRatingAsync(RatingModel rating)
        {
            var request = CreateRequest(HttpMethod.Post, "Rating");
            request.Content = JsonContent.Create(rating);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Remove a rating
        public async Task<bool> RemoveRatingAsync(int ratingId)
        {
            var request = CreateRequest(HttpMethod.Delete, $"Rating/{ratingId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
}