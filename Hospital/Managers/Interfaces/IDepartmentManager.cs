using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Managers
{
    public interface IDepartmentManager
    {
        List<DepartmentModel> GetDepartments();
        Task LoadDepartments();
    }
}
