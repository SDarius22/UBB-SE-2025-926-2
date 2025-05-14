using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.ApiClients
{
    public class AppointmentsApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:5035/api/";

        public AppointmentsApiService()
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

        // Get all appointments
        public async Task<List<AppointmentJointModel>> GetAllAppointmentsAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "Appointments");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<AppointmentJointModel>>();
        }

        // Get appointment by ID
        public async Task<AppointmentJointModel> GetAppointmentAsync(int appointmentId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Appointments/{appointmentId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppointmentJointModel>();
        }

        // Get appointments for a doctor by date
        public async Task<List<AppointmentJointModel>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            var request = CreateRequest(HttpMethod.Get, $"Appointments/doctor/{doctorId}/date/{date:yyyy-MM-dd}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<AppointmentJointModel>>();
        }

        // Get appointments for a specific doctor
        public async Task<List<AppointmentJointModel>> GetAppointmentsForDoctorAsync(int doctorId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Appointments/doctor/{doctorId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<AppointmentJointModel>>();
        }

        // Get appointments for a patient
        public async Task<List<AppointmentJointModel>> GetAppointmentsForPatientAsync(int patientId)
        {
            var request = CreateRequest(HttpMethod.Get, $"Appointments/patient/{patientId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<AppointmentJointModel>>();
        }

        // Add a new appointment
        public async Task<bool> AddAppointmentAsync(AppointmentModel appointment)
        {
            var request = CreateRequest(HttpMethod.Post, "Appointments");
            request.Content = JsonContent.Create(appointment);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Remove an appointment
        public async Task<bool> RemoveAppointmentAsync(int appointmentId)
        {
            var request = CreateRequest(HttpMethod.Delete, $"Appointments/{appointmentId}");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}