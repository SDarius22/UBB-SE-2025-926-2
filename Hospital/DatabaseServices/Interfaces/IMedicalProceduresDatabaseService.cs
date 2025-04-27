using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public interface IMedicalProceduresDatabaseService
    {
        Task<List<ProcedureModel>> GetProceduresByDepartmentId(int departmentId);
    }
}
