using Hospital.DatabaseServices;
using Hospital.Exceptions;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Managers
{
    public class AppointmentManager : IAppointmentManager
    {
        public List<AppointmentJointModel> Appointments { get; private set; }

        private readonly IAppointmentsDatabaseService _appointmentsDatabaseService;

        public AppointmentManager(IAppointmentsDatabaseService appointmentsDatabaseService)
        {
            _appointmentsDatabaseService = appointmentsDatabaseService;
            Appointments = new List<AppointmentJointModel>();
        }

        public List<AppointmentJointModel> GetAppointments()
        {
            return Appointments;
        }
        
        public async Task LoadDoctorAppointmentsOnDate(int doctorId, DateTime date)
        {
            try
            {
                List<AppointmentJointModel> appointments = await _appointmentsDatabaseService
                    .GetAppointmentsByDoctorAndDate(doctorId, date)
                    .ConfigureAwait(false);
                Appointments = new List<AppointmentJointModel>(appointments);
            }
            catch (Exception exception)
            {
                throw new Exception($"Error loading doctor appointments -\n {exception.Message}");
            }
        }

        public async Task LoadAppointmentsForPatient(int patientId)
        {
            try
            {
                List<AppointmentJointModel> appointments = await _appointmentsDatabaseService
                    .GetAppointmentsForPatient(patientId)
                    .ConfigureAwait(false);

                Appointments = new List<AppointmentJointModel>(
                    appointments.Where(appointment => appointment.DateAndTime > DateTime.Now && !appointment.Finished)
                );


                foreach (AppointmentJointModel appointment in Appointments)
                {
                    appointments.Add(appointment);
                }
            }
            catch (Exception exception)
            {
                throw new Exception($"Error loading appointments for patient {patientId}: {exception.Message}");
            }
        }

        public async Task<bool> RemoveAppointment(int appointmentId)
        {
            try
            {
                AppointmentJointModel appointment = await _appointmentsDatabaseService.GetAppointment(appointmentId);
                if (appointment == null)
                {
                    throw new AppointmentNotFoundException($"Appointment with ID {appointmentId} not found.");
                }

                if ((appointment.DateAndTime - DateTime.Now).TotalHours < 24)
                {
                    throw new CancellationNotAllowedException($"Appointment {appointmentId} is within 24 hours and cannot be canceled.");
                }

                if (!await _appointmentsDatabaseService.RemoveAppointmentFromDataBase(appointmentId))
                {
                    throw new DatabaseOperationException($"Failed to cancel appointment {appointmentId} due to a database error.");
                }

                return true;
            }
            catch (AppointmentNotFoundException)
            {
                throw;
            }
            catch (CancellationNotAllowedException)
            {
                throw;
            }
            catch (DatabaseOperationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new Exception($"Unexpected error removing appointment {appointmentId}: {exception.Message}", exception);
            }

        }

        public async Task LoadAppointmentsForDoctor(int doctorId)
        {
            try
            {
                List<AppointmentJointModel> appointments =
                    await _appointmentsDatabaseService.GetAppointmentsForDoctor(doctorId).ConfigureAwait(false);
                Appointments.Clear();

                foreach (AppointmentJointModel appointment in appointments)
                {
                    Appointments.Add(appointment);
                }
            }
            catch (Exception exception)
            {
                throw new Exception($"Error loading appointments for doctor {doctorId}: {exception.Message}");
            }
        }

        public async Task LoadAppointmentsByDoctorAndDate(int doctorId, DateTime date)
        {
            try
            {
                List<AppointmentJointModel> appointments = await _appointmentsDatabaseService
                    .GetAppointmentsByDoctorAndDate(doctorId, date)
                    .ConfigureAwait(false);
                Appointments.Clear();

                foreach (AppointmentJointModel appointment in appointments)
                {
                    Appointments.Add(appointment);
                }
            }
            catch (Exception exception)
            {
                throw new Exception($"Error loading appointments for doctor {doctorId} on date {date}: {exception.Message}");
            }
        }

        public async Task CreateAppointment(AppointmentModel newAppointment)
        {

            // Validate the doctor is available for the given time slot
            int doctorId = newAppointment.DoctorId;
            DateTime date = newAppointment.DateAndTime;
            List<AppointmentJointModel> existingAppointments = await _appointmentsDatabaseService
                                                                .GetAppointmentsByDoctorAndDate(doctorId, date)
                                                                .ConfigureAwait(false);

            bool isSlotTaken = existingAppointments.Any(appointment => appointment.DateAndTime == newAppointment.DateAndTime);

            if (isSlotTaken)
            {
                throw new AppointmentConflictException($"The selected time slot is already booked for doctor with id {doctorId}");
            }

            // Validate the patient doesn't have another appointment at the same time
            int patientId = newAppointment.PatientId;
            List<AppointmentJointModel> patientAppointments = await _appointmentsDatabaseService.GetAppointmentsForPatient(patientId).ConfigureAwait(false);

            bool isPatientBusy = patientAppointments.Any(appointment => appointment.DateAndTime == newAppointment.DateAndTime);

            if (isPatientBusy)
            {
                throw new AppointmentConflictException($"The patient with id {patientId} already has an appointment at the this time {newAppointment.DateAndTime}");
            }

            bool isInserted = await _appointmentsDatabaseService.AddAppointmentToDataBase(newAppointment).ConfigureAwait(false);

            if (!isInserted)
            {
                throw new DatabaseOperationException("Failed to save the appointment in the database");
            }
        }

        public static async Task MarkAppointmentAsCompletedInDatabase(int appointmentId)
        {
            throw new NotImplementedException();
        }

        public bool CanCancelAppointment(AppointmentJointModel appointment)
        {
            if (appointment == null)
            {
                return false;
            }

            return (appointment.DateAndTime - DateTime.Now).TotalHours >= 24;
        }
    }
}