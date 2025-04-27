using Hospital.Configs;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public class MedicalProceduresDatabaseService : IMedicalProceduresDatabaseService
    {
        private readonly ApplicationConfiguration _configuration;

        public MedicalProceduresDatabaseService()
        {
            _configuration = ApplicationConfiguration.GetInstance();
        }

        // This method will be used to get the procedures from the database
        public async Task<List<ProcedureModel>> GetProceduresByDepartmentId(int departmentId)
        {
            const string selectProceduresByDepartmentQuery = @"SELECT * FROM Procedures WHERE DepartmentId = @departmentId";

            try
            {
                using SqlConnection sqlConnection = new SqlConnection(_configuration.DatabaseConnection);
                await sqlConnection.OpenAsync().ConfigureAwait(false);

                //Prepare the command
                SqlCommand selectProceduresCommand = new SqlCommand(selectProceduresByDepartmentQuery, sqlConnection);
                selectProceduresCommand.Parameters.AddWithValue("@departmentId", departmentId);


                SqlDataReader reader = await selectProceduresCommand.ExecuteReaderAsync().ConfigureAwait(false);


                //Prepare the list of procedures
                List<ProcedureModel> procedures = new List<ProcedureModel>();

                //Read the data from the database
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    int procedureId = reader.GetInt32(0);
                    string procedureName = reader.GetString(1);
                    TimeSpan procedureDuration = reader.GetTimeSpan(2);
                    ProcedureModel medicalProcedure = new ProcedureModel(procedureId, departmentId, procedureName, procedureDuration);
                    procedures.Add(medicalProcedure);
                }
                return procedures;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Exception: {sqlException.Message}");
                return new List<ProcedureModel>();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Exception: {exception.Message}");
                return new List<ProcedureModel>();
            }
        }
    }
}
