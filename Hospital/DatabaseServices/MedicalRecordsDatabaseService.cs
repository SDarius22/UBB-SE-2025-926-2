using Hospital.Configs;
using Hospital.Exceptions;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public class MedicalRecordsDatabaseService : IMedicalRecordsDatabaseService
    {
        private readonly ApplicationConfiguration _configuration;

        public MedicalRecordsDatabaseService()
        {
            _configuration = ApplicationConfiguration.GetInstance();
        }

        public async Task<int> AddMedicalRecord(MedicalRecordModel medicalRecord)
        {
            DateTime recordDate = DateTime.Now;

            const string insertMedicalRecordQuery =
                "INSERT INTO MedicalRecords(DoctorId, PatientId, ProcedureId, Conclusion, DateAndTime) " +
                "OUTPUT INSERTED.MedicalRecordId " +
                "VALUES (@DoctorId, @PatientId, @ProcedureId, @Conclusion, @DateAndTime)";

            try
            {
                using var sqlConnection = new SqlConnection(_configuration.DatabaseConnection);
                await sqlConnection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                using var insertMedicalRecordCommand = new SqlCommand(insertMedicalRecordQuery, sqlConnection);

                insertMedicalRecordCommand.Parameters.AddWithValue("@DoctorId", medicalRecord.DoctorId);
                insertMedicalRecordCommand.Parameters.AddWithValue("@PatientId", medicalRecord.PatientId);
                insertMedicalRecordCommand.Parameters.AddWithValue("@ProcedureId", medicalRecord.ProcedureId);
                insertMedicalRecordCommand.Parameters.AddWithValue("@Conclusion", medicalRecord.Conclusion ?? (object)DBNull.Value);
                insertMedicalRecordCommand.Parameters.AddWithValue("@DateAndTime", recordDate); // Pass the record's date

                object result = await insertMedicalRecordCommand.ExecuteScalarAsync().ConfigureAwait(false);


                int medicalRecordId = result != null ? Convert.ToInt32(result) : -1;
                return medicalRecordId;
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

        public async Task<List<MedicalRecordJointModel>> GetMedicalRecordsForPatient(int patientId)
        {
            const string selectMedicalRecordForPatientQuery =
              "SELECT " +
              "     mr.MedicalRecordId, " +
              "     mr.PatientId, " +
              "     p.Name AS PatientName, " +
              "     mr.DoctorId, " +
              "     d.Name AS DoctorName, " +
              "     pr.DepartmentId, " +
              "     dept.DepartmentName, " +
              "     mr.ProcedureId, " +
              "     pr.ProcedureName, " +
              "     mr.DateAndTime, " +
              "     mr.Conclusion " +
              "FROM MedicalRecords mr " +
              "JOIN Users p ON mr.PatientId = p.UserId " +
              "JOIN Users d ON mr.DoctorId = d.UserId " +
              "JOIN Procedures pr ON mr.ProcedureId = pr.ProcedureId " +
              "JOIN Departments dept ON pr.DepartmentId = dept.DepartmentId " +
              "WHERE mr.PatientId = @PatientId";
            try
            {
                using var sqlConnection = new SqlConnection(_configuration.DatabaseConnection);

                // Open the database connection asynchronously
                await sqlConnection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                // Create a command to execute the SQL query
                using var selectMedicalRecordsForPatientCommand = new SqlCommand(selectMedicalRecordForPatientQuery, sqlConnection);

                // Add parameters to the query
                selectMedicalRecordsForPatientCommand.Parameters.AddWithValue("@PatientId", patientId);

                // Execute the query asynchronously and retrieve the MedicalRecord
                SqlDataReader reader = await selectMedicalRecordsForPatientCommand.ExecuteReaderAsync().ConfigureAwait(false);

                List<MedicalRecordJointModel> medicalRecords = new List<MedicalRecordJointModel>();

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    medicalRecords.Add(new MedicalRecordJointModel(
                        reader.GetInt32(0),     // MedicalRecordId
                        reader.GetInt32(1),     // PatientId
                        reader.GetString(2),    // PatientName
                        reader.GetInt32(3),     // DoctorId
                        reader.GetString(4),    // DoctorName
                        reader.GetInt32(5),     // DepartmentId
                        reader.GetString(6),    // DepartmentName
                        reader.GetInt32(7),     // ProcedureId
                        reader.GetString(8),    // ProcedureName
                        reader.GetDateTime(9),  // Date
                        reader.GetString(10)    // Conclusion
                    ));
                }



                if (medicalRecords.Count == 0)
                {
                    throw new MedicalRecordNotFoundException("No medical records found for the given patient.");
                }

                return medicalRecords;
            }
            catch (SqlException sqlException)
            {
                throw new MedicalRecordNotFoundException($"SQL Error: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new MedicalRecordNotFoundException($"General Error: {exception.Message}");
            }
        }

        public async Task<MedicalRecordJointModel> GetMedicalRecordById(int medicalRecordId)
        {
            const string selectMedicalRecordQuery =
              "SELECT " +
              "     mr.MedicalRecordId, " +
              "     mr.PatientId, " +
              "     p.Name AS PatientName, " +
              "     mr.DoctorId, " +
              "     d.Name AS DoctorName, " +
              "     pr.DepartmentId, " +
              "     dept.DepartmentName, " +
              "     mr.ProcedureId, " +
              "     pr.ProcedureName, " +
              "     mr.DateAndTime, " +
              "     mr.Conclusion " +
              "FROM MedicalRecords mr " +
              "JOIN Users p ON mr.PatientId = p.UserId " +
              "JOIN Users d ON mr.DoctorId = d.UserId " +
              "JOIN Procedures pr ON mr.ProcedureId = pr.ProcedureId " +
              "JOIN Departments dept ON pr.DepartmentId = dept.DepartmentId " +
              "WHERE MedicalRecordId = @MedicalRecordId";
            try
            {
                using var sqlConnection = new SqlConnection(_configuration.DatabaseConnection);

                // Open the database connection asynchronously
                await sqlConnection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                // Create a command to execute the SQL query
                using var selectMedicalRecordCommand = new SqlCommand(selectMedicalRecordQuery, sqlConnection);

                // Add parameters to the query
                selectMedicalRecordCommand.Parameters.AddWithValue("@MedicalRecordId", medicalRecordId);

                // Execute the query asynchronously and retrieve the MedicalRecord
                SqlDataReader reader = await selectMedicalRecordCommand.ExecuteReaderAsync().ConfigureAwait(false);
                MedicalRecordJointModel medicalRecord = null;

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    medicalRecord = new MedicalRecordJointModel(
                        reader.GetInt32(0),     // MedicalRecordId
                        reader.GetInt32(1),     // PatientId
                        reader.GetString(2),    // PatientName
                        reader.GetInt32(3),     // DoctorId
                        reader.GetString(4),    // DoctorName
                        reader.GetInt32(5),     // DepartmentId
                        reader.GetString(6),    // DepartmentName
                        reader.GetInt32(7),     // ProcedureId
                        reader.GetString(8),    // ProcedureName
                        reader.GetDateTime(9),  // Date
                        reader.GetString(10)     // Conclusion
                    );
                }

                if (medicalRecord == null)
                {
                    throw new MedicalRecordNotFoundException("No medical record found for the given ID.");
                }

                return medicalRecord;
            }
            catch (SqlException sqlException)
            {
                throw new MedicalRecordNotFoundException($"SQL Error: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new MedicalRecordNotFoundException($"General Error: {exception.Message}");
            }
        }

        public async Task<List<MedicalRecordJointModel>> GetMedicalRecordsForDoctor(int DoctorId)
        {
            const string selectMedicalRecordsForDoctorQuery =
              "SELECT " +
              "     mr.MedicalRecordId, " +
              "     mr.PatientId, " +
              "     p.Name AS PatientName, " +
              "     mr.DoctorId, " +
              "     d.Name AS DoctorName, " +
              "     pr.DepartmentId, " +
              "     dept.DepartmentName, " +
              "     mr.ProcedureId, " +
              "     pr.ProcedureName, " +
              "     mr.DateAndTime, " +
              "     mr.Conclusion " +
              "FROM MedicalRecords mr " +
              "JOIN Users p ON mr.PatientId = p.UserId " +
              "JOIN Users d ON mr.DoctorId = d.UserId " +
              "JOIN Procedures pr ON mr.ProcedureId = pr.ProcedureId " +
              "JOIN Departments dept ON pr.DepartmentId = dept.DepartmentId " +
              "WHERE DoctorId = @DoctorId";
            try
            {
                using var sqlConnection = new SqlConnection(_configuration.DatabaseConnection);

                // Open the database connection asynchronously
                await sqlConnection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                // Create a command to execute the SQL query
                using var selectMedicalRecordsForDoctorCommand = new SqlCommand(selectMedicalRecordsForDoctorQuery, sqlConnection);

                // Add parameters to the query
                selectMedicalRecordsForDoctorCommand.Parameters.AddWithValue("@DoctorId", DoctorId);

                // Execute the query asynchronously and retrieve the MedicalRecords
                SqlDataReader reader = await selectMedicalRecordsForDoctorCommand.ExecuteReaderAsync().ConfigureAwait(false);
                List<MedicalRecordJointModel> medicalRecords = new List<MedicalRecordJointModel>();

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    medicalRecords.Add(new MedicalRecordJointModel(
                        reader.GetInt32(0),     // MedicalRecordId
                        reader.GetInt32(1),     // PatientId
                        reader.GetString(2),    // PatientName
                        reader.GetInt32(3),     // DoctorId
                        reader.GetString(4),    // DoctorName
                        reader.GetInt32(5),     // DepartmentId
                        reader.GetString(6),    // DepartmentName
                        reader.GetInt32(7),     // ProcedureId
                        reader.GetString(8),    // ProcedureName
                        reader.GetDateTime(9),  // Date
                        reader.GetString(10)    // Conclusion
                    ));
                }

                if (medicalRecords.Count == 0)
                {
                    throw new MedicalRecordNotFoundException("No medical records found for the given doctor.");
                }

                return medicalRecords;
            }
            catch (SqlException sqlException)
            {
                throw new MedicalRecordNotFoundException($"SQL Error: {sqlException.Message}");
            }
            catch (Exception exception)
            {
                throw new MedicalRecordNotFoundException($"General Error: {exception.Message}");
            }
        }
    }
}