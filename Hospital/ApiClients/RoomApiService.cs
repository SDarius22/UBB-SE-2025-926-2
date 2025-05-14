using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.ApiClients
{
    public class RoomApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public RoomApiService()
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

        // Get all rooms
        public async Task<List<RoomModel>> GetRoomsAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "Room");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<RoomModel>>();
        }

        // Add a new room
        public async Task<bool> AddRoomAsync(RoomModel room)
        {
            var request = CreateRequest(HttpMethod.Post, "Room");
            request.Content = JsonContent.Create(room);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Update an existing room
        public async Task<bool> UpdateRoomAsync(int roomId, RoomModel room)
        {
            var request = CreateRequest(HttpMethod.Put, $"Room/{roomId}");
            request.Content = JsonContent.Create(room);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Delete a room
        public async Task<bool> DeleteRoomAsync(int roomId)
        {
            var request = CreateRequest(HttpMethod.Delete, $"Room/{roomId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a room exists
        public async Task<bool> DoesRoomExistAsync(int roomId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Room/room-exists/{roomId}");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Check if equipment exists
        public async Task<bool> DoesEquipmentExistAsync(int equipmentId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Room/exists/{equipmentId}");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Check if department exists
        public async Task<bool> DoesDepartmentExistAsync(int departmentId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Room/department-exists/{departmentId}");
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Error message: {errorMessage}");
            }
            else
            {
                Debug.WriteLine($"Department ID: {departmentId}, Exists: {await response.Content.ReadFromJsonAsync<bool>()}");
            }
            return response.IsSuccessStatusCode;
        }
    }
}