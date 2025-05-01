using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public interface IDepartmentsDatabaseService
    {
        Task<List<DepartmentModel>> GetDepartmentsFromDataBase();

        public Task<bool> AddDepartment(DepartmentModel department);

        public Task<bool> UpdateDepartment(DepartmentModel department);

        public Task<bool> DeleteDepartment(int departmentID);

        public Task<bool> DoesDepartmentExist(int departmentID);

        public Task<List<DepartmentModel>> GetDepartments();
    }
}
