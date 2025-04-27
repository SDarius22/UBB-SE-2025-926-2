using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Managers
{
    public interface IDoctorManager
    {
        Task LoadDoctors(int departmentId);
        List<DoctorJointModel> GetDoctorsWithRatings();
    }
}
