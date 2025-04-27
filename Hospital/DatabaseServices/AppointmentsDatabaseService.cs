// <copyright file="AppointmentsDatabaseService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Hospital.DatabaseServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Hospital.Configs;
    using Hospital.Exceptions;
    using Hospital.Models;
    using Microsoft.Data.SqlClient;

    /// <summary>
    /// Service for managing appointments in the database.
    /// </summary>
    public class AppointmentsDatabaseService : IAppointmentsDatabaseService
    {
        private readonly ApplicationConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppointmentsDatabaseService"/> class.
        /// </summary>
        public AppointmentsDatabaseService()
        {
            this.configuration = ApplicationConfiguration.GetInstance();
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

            const string InsertAppointmentQuery =
              "INSERT INTO Appointments (PatientId, DoctorId, DateAndTime, ProcedureId, Finished) " +
              "VALUES (@PatientId, @DoctorId, @DateAndTime, @ProcedureId, @Finished)";

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(this.configuration.DatabaseConnection);

                // Open the database connection asynchronously
                await sqlConnection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                // Create a command to execute the SQL query
                using SqlCommand sqlCommand = new SqlCommand(InsertAppointmentQuery, sqlConnection);

                // Add the parameters to the query with values from the appointment object
                sqlCommand.Parameters.AddWithValue("@PatientId", appointment.PatientId);
                sqlCommand.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
                sqlCommand.Parameters.AddWithValue("@DateAndTime", appointment.DateAndTime);
                sqlCommand.Parameters.AddWithValue("@ProcedureId", appointment.ProcedureId);
                sqlCommand.Parameters.AddWithValue("@Finished", appointment.Finished);

                // Execute the query asynchronously and check how many rows were affected
                int numberOfRowsAffectedByAddSqlCommand = await sqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

                sqlConnection.Close();

                // If at least one row was affected, the insert was successful
                return numberOfRowsAffectedByAddSqlCommand > 0;
            }
            catch (SqlException sqlException)
            {
                throw new DatabaseOperationException($"SQL Error: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new Exception($"General Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Retrieves all appointments from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The task result contains a list of appointments.</returns>
        /// <exception cref="DatabaseOperationException">Thrown when a database error occurs.</exception>
        public async Task<List<AppointmentJointModel>> GetAllAppointments()
        {
            const string SelectAppointmentsQuery = @"SELECT 
                    a.AppointmentId,
                    a.Finished,
                    a.DateAndTime,
                    d.DepartmentId,
                    d.DepartmentName,
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
                ORDER BY a.AppointmentId;";

            using DataTable appointmentsDataTable = new DataTable();

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(this.configuration.DatabaseConnection);
                await sqlConnection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                using SqlCommand sqlCommand = new SqlCommand(SelectAppointmentsQuery, sqlConnection);
                using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync().ConfigureAwait(false);
                await Task.Run(() => appointmentsDataTable.Load(reader)).ConfigureAwait(false);

                List<AppointmentJointModel> appointments = new List<AppointmentJointModel>();

                foreach (DataRow row in appointmentsDataTable.Rows)
                {
                    appointments.Add(new AppointmentJointModel(
                        (int)row["AppointmentId"],
                        (bool)row["Finished"],
                        (DateTime)row["DateAndTime"],
                        (int)row["DepartmentId"],
                        (string)row["DepartmentName"],
                        (int)row["DoctorId"],
                        (string)row["DoctorName"],
                        (int)row["PatientId"],
                        (string)row["PatientName"],
                        (int)row["ProcedureId"],
                        (string)row["ProcedureName"],
                        (TimeSpan)row["ProcedureDuration"]));
                }

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
        /// Retrieves appointments for a specific patient.
        /// </summary>
        /// <param name="patientId">The ID of the patient.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a list of appointments.</returns>
        /// <exception cref="DatabaseOperationException">Thrown when a database error occurs.</exception>
        public async Task<List<AppointmentJointModel>> GetAppointmentsForPatient(int patientId)
        {
            const string SelectAppointmentsByPatientIdQuery = @"SELECT 
                    a.AppointmentId,
                    a.Finished,
                    a.DateAndTime,
                    d.DepartmentId,
                    d.DepartmentName,
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
                WHERE p.PatientId = @PatientId
                ORDER BY a.DateAndTime;";

            using DataTable appointmentsForPatientDataTable = new DataTable();

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(this.configuration.DatabaseConnection);
                await sqlConnection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine($"Connected to DB. Fetching appointments for Patient ID: {patientId}");

                using SqlCommand sqlCommand = new SqlCommand(SelectAppointmentsByPatientIdQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@PatientId", patientId);

                using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync().ConfigureAwait(false);
                await Task.Run(() => appointmentsForPatientDataTable.Load(reader)).ConfigureAwait(false);

                List<AppointmentJointModel> appointmentsForPatient = new List<AppointmentJointModel>();
                Console.WriteLine($"Rows returned: {appointmentsForPatientDataTable.Rows.Count}");

                foreach (DataRow row in appointmentsForPatientDataTable.Rows)
                {
                    var appointment = new AppointmentJointModel(
                        (int)row["AppointmentId"],
                        (bool)row["Finished"],
                        (DateTime)row["DateAndTime"],
                        (int)row["DepartmentId"],
                        (string)row["DepartmentName"],
                        (int)row["DoctorId"],
                        (string)row["DoctorName"],
                        (int)row["PatientId"],
                        (string)row["PatientName"],
                        (int)row["ProcedureId"],
                        (string)row["ProcedureName"],
                        (TimeSpan)row["ProcedureDuration"]);

                    Console.WriteLine($"Appointment found: {appointment.AppointmentId} - {appointment.DateAndTime}");
                    appointmentsForPatient.Add(appointment);
                }

                return appointmentsForPatient;
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

            const string SelectAppointmentsByDoctorAndDateQuery = @"SELECT 
                    a.AppointmentId,
                    a.Finished,
                    a.DateAndTime,
                    d.DepartmentId,
                    d.DepartmentName,
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
                WHERE a.DoctorId = @DoctorId
                  AND CONVERT(DATE, a.DateAndTime) = @Date
                ORDER BY a.DateAndTime;";

            using DataTable appointmentsByDoctorAndDateDataTable = new DataTable();

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(this.configuration.DatabaseConnection);
                await sqlConnection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                using SqlCommand sqlCommand = new SqlCommand(SelectAppointmentsByDoctorAndDateQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@DoctorId", doctorId);
                sqlCommand.Parameters.AddWithValue("@Date", date.Date);

                using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync().ConfigureAwait(false);
                await Task.Run(() => appointmentsByDoctorAndDateDataTable.Load(reader)).ConfigureAwait(false);

                List<AppointmentJointModel> appointmentsByDoctorAndDate = new List<AppointmentJointModel>();

                foreach (DataRow row in appointmentsByDoctorAndDateDataTable.Rows)
                {
                    appointmentsByDoctorAndDate.Add(new AppointmentJointModel(
                        (int)row["AppointmentId"],
                        (bool)row["Finished"],
                        (DateTime)row["DateAndTime"],
                        (int)row["DepartmentId"],
                        (string)row["DepartmentName"],
                        (int)row["DoctorId"],
                        (string)row["DoctorName"],
                        (int)row["PatientId"],
                        (string)row["PatientName"],
                        (int)row["ProcedureId"],
                        (string)row["ProcedureName"],
                        (TimeSpan)row["ProcedureDuration"]));
                }

                return appointmentsByDoctorAndDate;
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
            const string SelectAppointmentsForDoctorQuery = @"SELECT 
                    a.AppointmentId,
                    a.Finished,
                    a.DateAndTime,
                    d.DepartmentId,
                    d.DepartmentName,
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
                WHERE a.DoctorId = @DoctorId
                ORDER BY a.DateAndTime;";

            using DataTable selectAppointmentsForDoctorDataTable = new DataTable();

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(this.configuration.DatabaseConnection);
                await sqlConnection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                using SqlCommand sqlCommand = new SqlCommand(SelectAppointmentsForDoctorQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@DoctorId", doctorId);

                using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync().ConfigureAwait(false);
                await Task.Run(() => selectAppointmentsForDoctorDataTable.Load(reader)).ConfigureAwait(false);

                List<AppointmentJointModel> appointmentsForDoctor = new List<AppointmentJointModel>();
                foreach (DataRow row in selectAppointmentsForDoctorDataTable.Rows)
                {
                    appointmentsForDoctor.Add(new AppointmentJointModel(
                        (int)row["AppointmentId"],
                        (bool)row["Finished"],
                        (DateTime)row["DateAndTime"],
                        (int)row["DepartmentId"],
                        (string)row["DepartmentName"],
                        (int)row["DoctorId"],
                        (string)row["DoctorName"],
                        (int)row["PatientId"],
                        (string)row["PatientName"],
                        (int)row["ProcedureId"],
                        (string)row["ProcedureName"],
                        (TimeSpan)row["ProcedureDuration"]));
                }

                return appointmentsForDoctor;
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
            const string GetAppointmentByAppointmentIdQuery = @"SELECT 
                    a.AppointmentId,
                    a.Finished,
                    a.DateAndTime,
                    d.DepartmentId,
                    d.DepartmentName,
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
                WHERE a.AppointmentId = @AppointmentId;";

            using DataTable dt = new DataTable();
            try
            {
                using SqlConnection sqlConnection = new SqlConnection(this.configuration.DatabaseConnection);
                await sqlConnection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                using SqlCommand sqlCommand = new SqlCommand(GetAppointmentByAppointmentIdQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@AppointmentId", appointmentId);

                using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync().ConfigureAwait(false);
                await Task.Run(() => dt.Load(reader)).ConfigureAwait(false);

                if (dt.Rows.Count == 0)
                {
                    return null;
                }

                return new AppointmentJointModel(
                    (int)dt.Rows[0]["AppointmentId"],
                    (bool)dt.Rows[0]["Finished"],
                    (DateTime)dt.Rows[0]["DateAndTime"],
                    (int)dt.Rows[0]["DepartmentId"],
                    (string)dt.Rows[0]["DepartmentName"],
                    (int)dt.Rows[0]["DoctorId"],
                    (string)dt.Rows[0]["DoctorName"],
                    (int)dt.Rows[0]["PatientId"],
                    (string)dt.Rows[0]["PatientName"],
                    (int)dt.Rows[0]["ProcedureId"],
                    (string)dt.Rows[0]["ProcedureName"],
                    (TimeSpan)dt.Rows[0]["ProcedureDuration"]);
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
                Console.WriteLine($"Checking if appointment ID {appointmentId} exists before deletion...");

                const string CheckAppointmentExistsQuery = "SELECT COUNT(*) FROM Appointments WHERE AppointmentId = @AppointmentId";

                using SqlConnection sqlConnection = new SqlConnection(this.configuration.DatabaseConnection);
                await sqlConnection.OpenAsync().ConfigureAwait(false);

                using SqlCommand checkAppointmentExistsCommand = new SqlCommand(CheckAppointmentExistsQuery, sqlConnection);
                checkAppointmentExistsCommand.Parameters.AddWithValue("@AppointmentId", appointmentId);

                int appointmentExists = (int)await checkAppointmentExistsCommand.ExecuteScalarAsync().ConfigureAwait(false);

                if (appointmentExists == 0)
                {
                    throw new DatabaseOperationException($"Appointment ID {appointmentId} does not exist in the database.");
                }

                Console.WriteLine($"Appointment ID {appointmentId} exists. Proceeding with deletion.");

                const string DeleteAppointmentQuery = "DELETE FROM Appointments WHERE AppointmentId = @AppointmentId";
                using SqlCommand deleteAppointmentCommand = new SqlCommand(DeleteAppointmentQuery, sqlConnection);
                deleteAppointmentCommand.Parameters.AddWithValue("@AppointmentId", appointmentId);

                int numberOfRowsAffectedByDeleteSqlCommand = await deleteAppointmentCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

                if (numberOfRowsAffectedByDeleteSqlCommand > 0)
                {
                    Console.WriteLine($"Successfully deleted appointment ID {appointmentId}.");
                    return true;
                }
                else
                {
                    throw new DatabaseOperationException($"Deletion failed for appointment ID {appointmentId}. No rows affected.");
                }
            }
            catch (SqlException sqlException)
            {
                throw new DatabaseOperationException($"SQL Error while deleting appointment: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new DatabaseOperationException($"General Error while deleting appointment: {exception.Message}");
            }
        }
    }
}
