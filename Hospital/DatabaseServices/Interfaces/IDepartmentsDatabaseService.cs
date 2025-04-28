using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public interface IDepartmentsDatabaseService
    {
        Task<List<DepartmentModel>> GetDepartmentsFromDataBase();
        public bool AddDepartment(DepartmentModel department);

        public bool UpdateDepartment(DepartmentModel department);

        public bool DeleteDepartment(int departmentID);

        public bool DoesDepartmentExist(int departmentID);

        public List<DepartmentModel> GetDepartments();
    }
}
