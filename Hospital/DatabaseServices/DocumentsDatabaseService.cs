using Hospital.Configs;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentModel = Hospital.Models.DocumentModel;

namespace Hospital.DatabaseServices
{
    public class DocumentDatabaseService : IDocumentDatabaseService
    {
        private readonly ApplicationConfiguration _configuration;

        public DocumentDatabaseService()
        {
            _configuration = ApplicationConfiguration.GetInstance();
        }

        public async Task<bool> UploadDocumentToDataBase(DocumentModel document)
        {
            const string insertDocumentQuery =
              "INSERT INTO Documents (MedicalRecordId, Files) " +
              "VALUES (@MedicalRecordId, @Files)";

            try
            {
                using var sqlConnection = new SqlConnection(_configuration.DatabaseConnection);

                // Open the database connection asynchronously
                await sqlConnection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                // Create a command to execute the SQL query
                using var insertDocumentCommand = new SqlCommand(insertDocumentQuery, sqlConnection);

                // Add the parameters to the query with values from the appointment object
                insertDocumentCommand.Parameters.AddWithValue("@MedicalRecordId", document.MedicalRecordId);
                insertDocumentCommand.Parameters.AddWithValue("@Files", document.Files);

                // Execute the query asynchronously and check how many rows were affected
                int numberOfRowsAffectedByInsertSqlCommand = await insertDocumentCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

                // Close DB Connection
                sqlConnection.Close();

                // If at least one row was affected, the insert was successful
                return numberOfRowsAffectedByInsertSqlCommand > 0;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Error: {sqlException.Message}");
                return false;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Error: {exception.Message}");
                return false;
            }
        }

        public async Task<List<DocumentModel>> GetDocumentsByMedicalRecordId(int medicalRecordId)
        {
            const string selectDocumentByMedicalRecordIdQuery =
                "SELECT * FROM Documents WHERE MedicalRecordId = @MedicalRecordId";
            try
            {
                using var sqlConnection = new SqlConnection(this._configuration.DatabaseConnection);

                // Open the database connection asynchronously
                await sqlConnection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                // Create a command to execute the SQL query
                using var selectDocumentCommand = new SqlCommand(selectDocumentByMedicalRecordIdQuery, sqlConnection);

                // Add the parameters to the query with values from the appointment object
                selectDocumentCommand.Parameters.AddWithValue("@MedicalRecordId", medicalRecordId);

                // Execute the query asynchronously and read the result
                using var reader = await selectDocumentCommand.ExecuteReaderAsync().ConfigureAwait(false);

                // Create a list to hold the documents
                List<DocumentModel> documents = new List<DocumentModel>();

                // Read all rows
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    // Create a new Document object with the values from the row
                    DocumentModel document = new DocumentModel(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetString(2));

                    // Add the document to the list
                    documents.Add(document);
                }

                // Close DB Connection
                sqlConnection.Close();

                // Return the list of Document objects
                return documents;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Error: {sqlException.Message}");
                return null;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Error: {exception.Message}");
                return null;
            }
        }
    }
}
