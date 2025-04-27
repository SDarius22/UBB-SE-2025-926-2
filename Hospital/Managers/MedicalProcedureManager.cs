using Hospital.DatabaseServices;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Managers
{
    public class MedicalProcedureManager : IMedicalProcedureManager
    {
        public static List<ProcedureModel> Procedures { get; private set; }
        private readonly IMedicalProceduresDatabaseService _medicalProcedureDatabaseService;

        public MedicalProcedureManager(IMedicalProceduresDatabaseService medicalProcedureDatabaseService)
        {
            _medicalProcedureDatabaseService = medicalProcedureDatabaseService;
            Procedures = new List<ProcedureModel>();
        }

        // This method will be used to get the procedures from the in memory repository
        public List<ProcedureModel> GetProcedures()
        {
            return Procedures;
        }

        // This method will be used to load the procedures from the database into the in memory repository
        public async Task LoadProceduresByDepartmentId(int departmentId)
        {
            try
            {
                Procedures.Clear();
                List<ProcedureModel> procedures = await _medicalProcedureDatabaseService.GetProceduresByDepartmentId(departmentId).ConfigureAwait(false);
                Procedures.Clear();
                foreach (ProcedureModel procedure in procedures)
                {
                    Procedures.Add(procedure);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error loading procedures: {exception.Message}");
                return;
            }
        }



    }
}
