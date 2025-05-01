using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.ApiClients
{
    public class DoctorApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public DoctorApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<List<DoctorJointModel>> GetDoctorsAsync()
        {
            var response = await _httpClient.GetAsync("Doctors");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DoctorJointModel>>();
        }
    }
}
