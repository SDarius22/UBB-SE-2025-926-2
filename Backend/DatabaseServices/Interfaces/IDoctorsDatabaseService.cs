using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.DatabaseServices
{
    public interface IDoctorsDatabaseService
    {
        Task<List<DoctorJointModel>> GetDoctorsByDepartment(int departmentId);

        public Task<bool> AddDoctor(DoctorJointModel doctor);

        public Task<bool> UpdateDoctor(DoctorJointModel doctor);

        public Task<bool> DeleteDoctor(int doctorID);

        public Task<bool> DoesDoctorExist(int doctorID);

        public Task<bool> IsUserAlreadyDoctor(int userID);

        public Task<bool> DoesUserExist(int userID);

        public Task<bool> IsUserDoctor(int userID);

        public Task<bool> DoesDepartmentExist(int departmentID);

        public Task<bool> UserExistsInDoctors(int userID, int doctorID);

        public List<ShiftModel> GetShiftsForCurrentMonth(int doctorID);

        public Task<double> ComputeDoctorSalary(int doctorID);

        public Task<List<DoctorJointModel>> GetDoctors();
    }
}
