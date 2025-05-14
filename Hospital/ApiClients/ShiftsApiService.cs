using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.ApiClients
{
    public class ShiftsApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public ShiftsApiService()
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

        // Get all shifts
        public async Task<List<ShiftModel>> GetShiftsAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "Shifts");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ShiftModel>>();
        }

        // Get shifts by doctor ID
        public async Task<List<ShiftModel>> GetShiftsByDoctorIdAsync(int doctorId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Shifts/doctor/{doctorId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ShiftModel>>();
        }

        // Get doctor's daytime shifts
        public async Task<List<ShiftModel>> GetDoctorDaytimeShiftsAsync(int doctorId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Shifts/doctor/{doctorId}/daytime");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ShiftModel>>();
        }

        // Add a new shift
        public async Task<bool> AddShiftAsync(ShiftModel shift)
        {
            var request = CreateRequest(HttpMethod.Post, "Shifts");
            request.Content = JsonContent.Create(shift);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Update an existing shift
        public async Task<bool> UpdateShiftAsync(int shiftId, ShiftModel shift)
        {
            var request = CreateRequest(HttpMethod.Put, $"Shifts/{shiftId}");
            request.Content = JsonContent.Create(shift);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Delete a shift
        public async Task<bool> DeleteShiftAsync(int shiftId)
        {
            var request = CreateRequest(HttpMethod.Delete, $"Shifts/{shiftId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a shift exists
        public async Task<bool> DoesShiftExistAsync(int shiftId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Shifts/exists/{shiftId}");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            else
            {
                throw new Exception("Error checking shift existence");
            }
        }
    }
}