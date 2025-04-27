using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Managers
{
    public interface IMedicalProcedureManager
    {
        List<ProcedureModel> GetProcedures();
        Task LoadProceduresByDepartmentId(int departmentId);
    }
}
