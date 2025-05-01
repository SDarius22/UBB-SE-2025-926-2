using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices.Interfaces
{
    public interface IMedicalProceduresDatabaseService
    {
        Task<List<ProcedureModel>> GetProceduresByDepartmentId(int departmentId);
    }
}
