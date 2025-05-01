using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Managers
{
    public interface IMedicalProcedureManager
    {
        List<ProcedureModel> GetProcedures();
        Task LoadProceduresByDepartmentId(int departmentId);
    }
}
