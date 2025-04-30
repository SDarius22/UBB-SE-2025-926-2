using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public interface IDepartmentsDatabaseService
    {
        Task<List<DepartmentModel>> GetDepartmentsFromDataBase();
    }
}
