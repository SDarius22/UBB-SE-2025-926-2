using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.ApiClients
{
    public class DoctorInformationApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public DoctorInformationApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        // Get doctor information by doctorId
        public async Task<DoctorInformationModel> GetDoctorInformationAsync(int doctorId)
        {
            var response = await _httpClient.GetAsync($"DoctorInformation/{doctorId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DoctorInformationModel>();
        }
    }
}
