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
    public class ShiftsApiService : IShiftsApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public ShiftsApiService(IHttpContextAccessor accessor)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            var token = accessor.HttpContext?.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<ShiftModel>> GetShiftsAsync()
        {
            var response = await _httpClient.GetAsync("Shifts");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ShiftModel>>();
        }

        public async Task<ShiftModel> GetShiftAsync(int id)
        {
            var response = await _httpClient.GetAsync("Shifts");
            response.EnsureSuccessStatusCode();
            var shifts = await response.Content.ReadFromJsonAsync<List<ShiftModel>>();

            return shifts.FirstOrDefault(s => s.ShiftID == id);
        }


        public async Task<List<ShiftModel>> GetShiftsByDoctorIdAsync(int doctorId)
        {
            var response = await _httpClient.GetAsync($"Shifts/doctor/{doctorId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ShiftModel>>();
        }

        public async Task<List<ShiftModel>> GetDoctorDaytimeShiftsAsync(int doctorId)
        {
            var response = await _httpClient.GetAsync($"Shifts/doctor/{doctorId}/daytime");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ShiftModel>>();
        }

        public async Task<bool> AddShiftAsync(ShiftModel shift)
        {
            var response = await _httpClient.PostAsJsonAsync("Shifts", shift);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateShiftAsync(int shiftId, ShiftModel shift)
        {
            var response = await _httpClient.PutAsJsonAsync($"Shifts/{shiftId}", shift);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DeleteShiftAsync(int shiftId)
        {
            var response = await _httpClient.DeleteAsync($"Shifts/{shiftId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DoesShiftExistAsync(int shiftId)
        {
            var response = await _httpClient.GetAsync($"Shifts/exists/{shiftId}");
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
