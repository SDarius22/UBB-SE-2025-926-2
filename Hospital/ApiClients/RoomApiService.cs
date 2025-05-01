using System;
using System.Collections.Generic;
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

        public async Task<List<RoomModel>> GetRoomsAsync()
        {
            var response = await _httpClient.GetAsync("Room");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<RoomModel>>();
        }

        public async Task<bool> AddRoomAsync(RoomModel room)
        {
            var response = await _httpClient.PostAsJsonAsync("Room", room);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateRoomAsync(int roomId, RoomModel room)
        {
            var response = await _httpClient.PutAsJsonAsync($"Room/{roomId}", room);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DeleteRoomAsync(int roomId)
        {
            var response = await _httpClient.DeleteAsync($"Room/{roomId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        // Check if a room exists
        public async Task<bool> DoesRoomExistAsync(int roomId)
        {
            var response = await _httpClient.GetAsync($"Room/{roomId}/exists");
            return response.IsSuccessStatusCode;
        }

        // Check if equipment exists
        public async Task<bool> DoesEquipmentExistAsync(int equipmentId)
        {
            var response = await _httpClient.GetAsync($"Equipment/{equipmentId}/exists");
            return response.IsSuccessStatusCode;
        }

        // Check if department exists
        public async Task<bool> DoesDepartmentExistAsync(int departmentId)
        {
            var response = await _httpClient.GetAsync($"Department/{departmentId}/exists");
            return response.IsSuccessStatusCode;
        }
    }
}