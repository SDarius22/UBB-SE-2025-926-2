using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend.ApiClients.Interface;
using Frontend.Models;

namespace Hospital.ApiClients
{
    public class ScheduleApiService : IScheduleApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public ScheduleApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<List<ScheduleModel>> GetSchedulesAsync()
        {
            var response = await _httpClient.GetAsync("Schedule");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ScheduleModel>>();
        }

        public async Task<ScheduleModel> GetScheduleAsync(int id)
        {
            var response = await _httpClient.GetAsync("Schedule");
            response.EnsureSuccessStatusCode();
            var schedules = await response.Content.ReadFromJsonAsync<List<ScheduleModel>>();

            return schedules.FirstOrDefault(s => s.ScheduleId == id);
        }

        public async Task<bool> AddScheduleAsync(ScheduleModel schedule)
        {
            var response = await _httpClient.PostAsJsonAsync("Schedule", schedule);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> UpdateScheduleAsync(int scheduleId, ScheduleModel schedule)
        {
            var response = await _httpClient.PutAsJsonAsync($"Schedule/{scheduleId}", schedule);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            var response = await _httpClient.DeleteAsync($"Schedule/{scheduleId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }


        public async Task<bool> DoesScheduleExistAsync(int scheduleId)
        {
            var response = await _httpClient.GetAsync($"Schedule/exists/{scheduleId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DoesDoctorExistAsync(int doctorId)
        {
            var response = await _httpClient.GetAsync($"Schedule/doctor-exists/{doctorId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DoesShiftExistAsync(int shiftId)
        {
            var response = await _httpClient.GetAsync($"Schedule/shift-exists/{shiftId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

    }
}