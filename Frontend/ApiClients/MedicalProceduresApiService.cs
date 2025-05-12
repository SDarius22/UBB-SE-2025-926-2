using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend.Models;

namespace Hospital.ApiClients
{
    public class MedicalProceduresApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public MedicalProceduresApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<List<ProcedureModel>> GetProceduresByDepartmentAsync(int departmentId)
        {
            var response = await _httpClient.GetAsync($"MedicalProcedures/department/{departmentId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ProcedureModel>>();
        }
    }
}