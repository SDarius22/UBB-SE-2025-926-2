using System;
using System.Data;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Windows.System;

namespace Hospital.ApiClients
{
    public class UserApiService
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

        public async Task<string> Login(string username, string password)
        {
            var response = await _httpClient.GetAsync($"User/login?username={username}&password={password}");
            try
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}