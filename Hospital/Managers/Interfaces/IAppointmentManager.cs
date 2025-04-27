using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.Managers
{
    public interface IAppointmentManager
    {
        List<AppointmentJointModel> Appointments { get; }
        List<AppointmentJointModel> GetAppointments();
        Task LoadDoctorAppointmentsOnDate(int doctorId, DateTime date);
        Task LoadAppointmentsForPatient(int patientId);
        Task<bool> RemoveAppointment(int appointmentId);
        Task LoadAppointmentsForDoctor(int doctorId);
        Task LoadAppointmentsByDoctorAndDate(int doctorId, DateTime date);
        Task CreateAppointment(AppointmentModel newAppointment);

        public bool CanCancelAppointment(AppointmentJointModel appointment);
    }
} 