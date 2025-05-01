using Hospital.ApiClients;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Managers
{
    public class DepartmentManager : IDepartmentManager
    {
        public static List<DepartmentModel> Departments { get; private set; }

        private readonly DepartmentsApiService _departmentDatabaseService;

        public DepartmentManager(DepartmentsApiService departmentDatabaseService)
        {
            _departmentDatabaseService = departmentDatabaseService;
            Departments = new List<DepartmentModel>();
        }

        // This method will be used to get the departments from the in memory repository
        public List<DepartmentModel> GetDepartments()
        {
            return Departments;
        }

        // This method will be used to load the departments from the database into the in memory repository
        public async Task LoadDepartments()
        {

            Departments.Clear();
            List<DepartmentModel> departmentList = await _departmentDatabaseService.GetDepartmentsAsync().ConfigureAwait(false);
            foreach (DepartmentModel department in departmentList)
            {
                Departments.Add(department);
            }
        }

    }
}
