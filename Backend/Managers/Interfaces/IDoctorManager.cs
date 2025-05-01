using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Managers
{
    public interface IDoctorManager
    {
        Task LoadDoctors(int departmentId);
        List<DoctorJointModel> GetDoctorsWithRatings();
    }
}
