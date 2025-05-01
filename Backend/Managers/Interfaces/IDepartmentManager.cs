using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Managers
{
    public interface IDepartmentManager
    {
        List<DepartmentModel> GetDepartments();
        Task LoadDepartments();
    }
}
