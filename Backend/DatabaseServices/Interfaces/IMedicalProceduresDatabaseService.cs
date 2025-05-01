using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.DatabaseServices.Interfaces
{
    public interface IMedicalProceduresDatabaseService
    {
        Task<List<ProcedureModel>> GetProceduresByDepartmentId(int departmentId);
    }
}
