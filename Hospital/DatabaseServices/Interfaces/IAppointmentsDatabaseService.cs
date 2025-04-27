namespace Hospital.DatabaseServices
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Hospital.Models;
    public interface IAppointmentsDatabaseService
    {
        Task<bool> AddAppointmentToDataBase(AppointmentModel appointment);
        Task<bool> RemoveAppointmentFromDataBase(int appointmentId);
        Task<AppointmentJointModel> GetAppointment(int appointmentId);
        Task<List<AppointmentJointModel>> GetAppointmentsByDoctorAndDate(int doctorId, DateTime date);
        Task<List<AppointmentJointModel>> GetAppointmentsForPatient(int patientId);
        Task<List<AppointmentJointModel>> GetAppointmentsForDoctor(int doctorId);
        Task<List<AppointmentJointModel>> GetAllAppointments();
    }
} 