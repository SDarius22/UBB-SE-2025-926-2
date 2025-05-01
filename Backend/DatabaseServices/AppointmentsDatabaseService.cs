// <copyright file="AppointmentsDatabaseService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Backend.DatabaseServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Backend.Configs;
    using Backend.DbContext;
    using Backend.Exceptions;
    using Backend.Models;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Service for managing appointments in the database.
    /// </summary>
    public class AppointmentsDatabaseService : IAppointmentsDatabaseService
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppointmentsDatabaseService"/> class.
        /// </summary>
        public AppointmentsDatabaseService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new appointment to the database.
        /// </summary>
        /// <param name="appointment">The appointment to add.</param>
        /// <returns>A task representing the asynchronous operation. The task result is true if the appointment was added successfully.</returns>
        /// <exception cref="InvalidAppointmentException">Thrown when the appointment date is in the past.</exception>
        /// <exception cref="DatabaseOperationException">Thrown when a database error occurs.</exception>
        public async Task<bool> AddAppointmentToDataBase(AppointmentModel appointment)
        {
            // Validate that the appointment is not in the past
            if (appointment.DateAndTime < DateTime.Now)
            {
                throw new InvalidAppointmentException("Cannot create appointments in the past");
            }

            try
            {
                var entity = new AppointmentModel(
                    appointment.AppointmentId,
                    appointment.DoctorId,
                    appointment.PatientId,
                    appointment.DateAndTime,
                    appointment.Finished,
                    appointment.ProcedureId);

                await _context.Appointments.AddAsync(entity);
                await _context.SaveChangesAsync();

                appointment.AppointmentId = entity.AppointmentId;

                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves all appointments from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The task result contains a list of appointments.</returns>
        /// <exception cref="DatabaseOperationException">Thrown when a database error occurs.</exception>
        public async Task<List<AppointmentJointModel>> GetAllAppointments()
        {
            try
            {
                return await _context.Appointments
                    .Join(
                        _context.DoctorJoints,
                        a => a.DoctorId,
                        doc => doc.DoctorId,
                        (a, doc) => new { Appointment = a, Doctor = doc }
                    )
                    .Join(
                        _context.Users,
                        ad => ad.Doctor.UserId,
                        u1 => u1.UserID,
                        (ad, u1) => new { ad.Appointment, ad.Doctor, DoctorUser = u1 }
                    )
                    .Join(
                        _context.Departments,
                        add => add.Doctor.DepartmentId,
                        d => d.DepartmentID,
                        (add, d) => new { add.Appointment, add.Doctor, add.DoctorUser, Department = d }
                    )
                    .Join(
                        _context.PatientJoints,
                        addd => addd.Appointment.PatientId,
                        p => p.PatientId,
                        (addd, p) => new { addd.Appointment, addd.Doctor, addd.DoctorUser, addd.Department, Patient = p }
                    )
                    .Join(
                        _context.Users,
                        addp => addp.Patient.UserId,
                        u2 => u2.UserID,
                        (addp, u2) => new { addp.Appointment, addp.Doctor, addp.DoctorUser, addp.Department, addp.Patient, PatientUser = u2 }
                    )
                    .Join(
                        _context.Procedures,
                        addpu => addpu.Appointment.ProcedureId,
                        pr => pr.ProcedureId,
                        (addpu, pr) => new AppointmentJointModel
                        {
                            AppointmentId = addpu.Appointment.AppointmentId,
                            Finished = addpu.Appointment.Finished,
                            DateAndTime = addpu.Appointment.DateAndTime,
                            DepartmentId = addpu.Department.DepartmentID,
                            Name = addpu.Department.Name,
                            DoctorId = addpu.Doctor.DoctorId,
                            DoctorName = addpu.DoctorUser.Name,
                            PatientId = addpu.Patient.PatientId,
                            PatientName = addpu.PatientUser.Name,
                            ProcedureId = pr.ProcedureId,
                            ProcedureName = pr.ProcedureName,
                            ProcedureDuration = pr.ProcedureDuration
                        }
                    )
                    .OrderBy(a => a.AppointmentId)
                    .ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseOperationException(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new DatabaseOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves appointments for a specific patient.
        /// </summary>
        /// <param name="patientId">The ID of the patient.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a list of appointments.</returns>
        /// <exception cref="DatabaseOperationException">Thrown when a database error occurs.</exception>
        public async Task<List<AppointmentJointModel>> GetAppointmentsForPatient(int patientId)
        {
            try
            {
                var str = @"
                    SELECT 
                        a.AppointmentId,
                        a.Finished,
                        a.DateAndTime,
                        d.DepartmentId,
                        d.Name,
                        doc.DoctorId,
                        u1.Name as DoctorName,
                        p.PatientId,
                        u2.Name as PatientName,
                        pr.ProcedureId,
                        pr.ProcedureName,
                        pr.ProcedureDuration
                    FROM Appointments a
                    JOIN Doctors doc ON a.DoctorId = doc.DoctorId
                    JOIN Users u1 ON doc.UserId = u1.UserId
                    JOIN Departments d ON doc.DepartmentId = d.DepartmentId
                    JOIN Patients p ON a.PatientId = p.PatientId
                    JOIN Users u2 ON p.UserId = u2.UserId
                    JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
                    WHERE p.PatientId = {0}
                    ORDER BY a.DateAndTime";

                var query = FormattableStringFactory.Create(str, patientId);

                var appointments = await Task.Run(() =>
                    _context.Database.SqlQuery<AppointmentJointModel>(query).ToList());

                return appointments;
            }
            catch (SqlException sqlException)
            {
                throw new DatabaseOperationException($"SQL Error: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException($"General Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Retrieves appointments for a specific doctor on a specific date.
        /// </summary>
        /// <param name="doctorId">The ID of the doctor.</param>
        /// <param name="date">The date to check appointments for.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a list of appointments.</returns>
        /// <exception cref="DatabaseOperationException">Thrown when a database error occurs.</exception>
        /// <exception cref="InvalidAppointmentException">Thrown when the date is in the past.</exception>
        public async Task<List<AppointmentJointModel>> GetAppointmentsByDoctorAndDate(int doctorId, DateTime date)
        {
            if (doctorId < 0)
            {
                throw new DatabaseOperationException($"Doctor ID {doctorId} is invalid.");
            }

            if (date < DateTime.Now)
            {
                throw new InvalidAppointmentException($"Date {date} is in the past.");
            }

            try
            {
                var query = FormattableStringFactory.Create(@"
                    SELECT 
                        a.AppointmentId,
                        a.Finished,
                        a.DateAndTime,
                        d.DepartmentId,
                        d.Name,
                        doc.DoctorId,
                        u1.Name as DoctorName,
                        p.PatientId,
                        u2.Name as PatientName,
                        pr.ProcedureId,
                        pr.ProcedureName,
                        pr.ProcedureDuration
                    FROM Appointments a
                    JOIN Doctors doc ON a.DoctorId = doc.DoctorId
                    JOIN Users u1 ON doc.UserId = u1.UserId
                    JOIN Departments d ON doc.DepartmentId = d.DepartmentId
                    JOIN Patients p ON a.PatientId = p.PatientId
                    JOIN Users u2 ON p.UserId = u2.UserId
                    JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
                    WHERE a.DoctorId = {0}
                      AND CONVERT(DATE, a.DateAndTime) = {1}
                    ORDER BY a.DateAndTime",
                            doctorId,
                            date.Date);

                var appointments = await Task.Run(() =>
                    _context.Database.SqlQuery<AppointmentJointModel>(query).ToList());

                return appointments;
            }
            catch (SqlException sqlException)
            {
                throw new DatabaseOperationException($"SQL Error: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException($"General Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Retrieves appointments for a specific doctor.
        /// </summary>
        /// <param name="doctorId">The ID of the doctor.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a list of appointments.</returns>
        /// <exception cref="DatabaseOperationException">Thrown when a database error occurs.</exception>
        public async Task<List<AppointmentJointModel>> GetAppointmentsForDoctor(int doctorId)
        {
            try
            {
                var query = FormattableStringFactory.Create(@"
            SELECT 
                a.AppointmentId,
                a.Finished,
                a.DateAndTime,
                d.DepartmentId,
                d.Name,
                doc.DoctorId,
                u1.Name as DoctorName,
                p.PatientId,
                u2.Name as PatientName,
                pr.ProcedureId,
                pr.ProcedureName,
                pr.ProcedureDuration
            FROM Appointments a
            JOIN Doctors doc ON a.DoctorId = doc.DoctorId
            JOIN Users u1 ON doc.UserId = u1.UserId
            JOIN Departments d ON doc.DepartmentId = d.DepartmentId
            JOIN Patients p ON a.PatientId = p.PatientId
            JOIN Users u2 ON p.UserId = u2.UserId
            JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
            WHERE a.DoctorId = {0}
            ORDER BY a.DateAndTime",
                    doctorId);

                var appointments = await Task.Run(() =>
                    _context.Database.SqlQuery<AppointmentJointModel>(query).ToList());

                return appointments;
            }
            catch (SqlException sqlException)
            {
                throw new DatabaseOperationException($"SQL Error: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException($"General Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Retrieves a specific appointment by its ID.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the appointment, or null if not found.</returns>
        /// <exception cref="DatabaseOperationException">Thrown when a database error occurs.</exception>
        public async Task<AppointmentJointModel> GetAppointment(int appointmentId)
        {
            try
            {
                var query = FormattableStringFactory.Create(@"
            SELECT 
                a.AppointmentId,
                a.Finished,
                a.DateAndTime,
                d.DepartmentId,
                d.Name,
                doc.DoctorId,
                u1.Name as DoctorName,
                p.PatientId,
                u2.Name as PatientName,
                pr.ProcedureId,
                pr.ProcedureName,
                pr.ProcedureDuration
            FROM Appointments a
            JOIN Doctors doc ON a.DoctorId = doc.DoctorId
            JOIN Users u1 ON doc.UserId = u1.UserId
            JOIN Departments d ON doc.DepartmentId = d.DepartmentId
            JOIN Patients p ON a.PatientId = p.PatientId
            JOIN Users u2 ON p.UserId = u2.UserId
            JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
            WHERE a.AppointmentId = {0}",
                    appointmentId);

                var appointment = await Task.Run(() =>
                    _context.Database.SqlQuery<AppointmentJointModel>(query).FirstOrDefault());

                return appointment;
            }
            catch (SqlException sqlException)
            {
                throw new DatabaseOperationException($"SQL Error: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException($"General Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Removes an appointment from the database.
        /// </summary>
        /// <param name="appointmentId">The ID of the appointment to remove.</param>
        /// <returns>A task representing the asynchronous operation. The task result is true if the appointment was removed successfully.</returns>
        /// <exception cref="DatabaseOperationException">Thrown when a database error occurs or the appointment does not exist.</exception>
        public async Task<bool> RemoveAppointmentFromDataBase(int appointmentId)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(appointmentId);
                if (appointment == null) return false;

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
