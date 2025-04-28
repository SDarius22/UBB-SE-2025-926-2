using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.DatabaseServices
{
    public interface IDoctorsDatabaseService
    {
        Task<List<DoctorJointModel>> GetDoctorsByDepartment(int departmentId);
        public bool AddDoctor(DoctorJointModel doctor);
        public bool UpdateDoctor(DoctorJointModel doctor);
        public bool DeleteDoctor(int doctorID);
        public bool DoesDoctorExist(int doctorID);
        public bool IsUserAlreadyDoctor(int userID);
        public bool DoesDepartmentExist(int departmentID);
        public bool UserExistsInDoctors(int userID, int doctorID);
        public List<ShiftModel> GetShiftsForCurrentMonth(int doctorID);
        public double ComputeDoctorSalary(int doctorID);
        public List<DoctorJointModel> GetDoctors();
    }
}
