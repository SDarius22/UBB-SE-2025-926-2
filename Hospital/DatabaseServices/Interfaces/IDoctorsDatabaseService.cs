using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public interface IDoctorsDatabaseService
    {
        Task<List<DoctorJointModel>> GetDoctorsByDepartment(int departmentId);
    }
}
