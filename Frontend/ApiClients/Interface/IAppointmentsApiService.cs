using Frontend.Models;

namespace Frontend.ApiClients.Interface;

public interface IAppointmentsApiService
{
    // Get all appointments
    public Task<List<AppointmentJointModel>> GetAllAppointmentsAsync();

    // Get appointment by ID
    public Task<AppointmentJointModel> GetAppointmentAsync(int appointmentId);

    // Get appointments for a doctor by date
    public Task<List<AppointmentJointModel>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date);

    // Get appointments for a specific doctor
    public Task<List<AppointmentJointModel>> GetAppointmentsForDoctorAsync(int doctorId);

    // Get appointments for a patient
    public Task<List<AppointmentJointModel>> GetAppointmentsForPatientAsync(int patientId);

    // Add a new appointment
    public Task<bool> AddAppointmentAsync(AppointmentModel appointment);

    // Remove an appointment
    public Task<bool> RemoveAppointmentAsync(int appointmentId);
}
