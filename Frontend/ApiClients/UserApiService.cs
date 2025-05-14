using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend.ApiClients.Interface;

namespace Hospital.ApiClients
{
    public class UserApiService : IUserApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public UserApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<bool> CheckUserRoleAsync(int userId, string role)
        {
            var response = await _httpClient.GetAsync($"User/check-role/{userId}?role={role}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
}