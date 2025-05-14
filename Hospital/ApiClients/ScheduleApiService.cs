using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.ApiClients
{
    public class ScheduleApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public ScheduleApiService()
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

        // Get all schedules
        public async Task<List<ScheduleModel>> GetSchedulesAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "Schedule");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ScheduleModel>>();
        }

        // Add a new schedule
        public async Task<bool> AddScheduleAsync(ScheduleModel schedule)
        {
            var request = CreateRequest(HttpMethod.Post, "Schedule");
            request.Content = JsonContent.Create(schedule);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Update an existing schedule
        public async Task<bool> UpdateScheduleAsync(int scheduleId, ScheduleModel schedule)
        {
            var request = CreateRequest(HttpMethod.Put, $"Schedule/{scheduleId}");
            request.Content = JsonContent.Create(schedule);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Delete a schedule
        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            var request = CreateRequest(HttpMethod.Delete, $"Schedule/{scheduleId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a schedule exists
        public async Task<bool> DoesScheduleExistAsync(int scheduleId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Schedule/exists/{scheduleId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a doctor exists
        public async Task<bool> DoesDoctorExistAsync(int doctorId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Schedule/doctor-exists/{doctorId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a shift exists
        public async Task<bool> DoesShiftExistAsync(int shiftId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Schedule/shift-exists/{shiftId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
}