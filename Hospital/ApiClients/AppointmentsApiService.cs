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

        // Get all appointments
        public async Task<List<AppointmentJointModel>> GetAllAppointmentsAsync()
        {
            var response = await _httpClient.GetAsync("Appointments");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<AppointmentJointModel>>();
        }

        // Get appointment by ID
        public async Task<AppointmentJointModel> GetAppointmentAsync(int appointmentId)
        {
            var response = await _httpClient.GetAsync($"Appointments/{appointmentId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppointmentJointModel>();
        }

        // Get appointments for a doctor by date
        public async Task<List<AppointmentJointModel>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            var response = await _httpClient.GetAsync($"Appointments/doctor/{doctorId}/date/{date:yyyy-MM-dd}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<AppointmentJointModel>>();
        }

        // Get appointments for a specific doctor
        public async Task<List<AppointmentJointModel>> GetAppointmentsForDoctorAsync(int doctorId)
        {
            var response = await _httpClient.GetAsync($"Appointments/doctor/{doctorId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<AppointmentJointModel>>();
        }

        // Get appointments for a patient
        public async Task<List<AppointmentJointModel>> GetAppointmentsForPatientAsync(int patientId)
        {
            var response = await _httpClient.GetAsync($"Appointments/patient/{patientId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<AppointmentJointModel>>();
        }

        // Add a new appointment
        public async Task<bool> AddAppointmentAsync(AppointmentModel appointment)
        {
            var response = await _httpClient.PostAsJsonAsync("Appointments", appointment);
            return response.IsSuccessStatusCode;
        }

        // Remove an appointment
        public async Task<bool> RemoveAppointmentAsync(int appointmentId)
        {
            var response = await _httpClient.DeleteAsync($"Appointments/{appointmentId}");
            return response.IsSuccessStatusCode;
        }
    }
}
