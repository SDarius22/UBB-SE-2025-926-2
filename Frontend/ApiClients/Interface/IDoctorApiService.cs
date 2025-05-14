using System.Diagnostics;
using Frontend.Models;

namespace Frontend.ApiClients.Interface;

public interface IDoctorApiService
{
    // Get all doctors
    public Task<List<DoctorJointModel>> GetDoctorsAsync();

    public Task<DoctorJointModel> GetDoctorAsync(int id);

    // Get doctors by department
    public Task<List<DoctorJointModel>> GetDoctorsByDepartmentAsync(int departmentId);

    // Add a new doctor
    public Task<bool> AddDoctorAsync(DoctorJointModel doctor);

    // Update an existing doctor
    public Task<bool> UpdateDoctorAsync(int doctorId, DoctorJointModel doctor);

    // Delete a doctor
    public Task<bool> DeleteDoctorAsync(int doctorId);

    // Check if a user is already a doctor
    public Task<bool> IsUserAlreadyDoctorAsync(int userId);

    // Check if a user exists in the system
    public Task<bool> DoesUserExistAsync(int userId);

    // Check if a user is a doctor
    public Task<bool> IsUserDoctorAsync(int userId);

    // Check if a department exists
    public Task<bool> DoesDepartmentExistAsync(int departmentId);

    // Check if a user exists in the doctors table
    public Task<bool> UserExistsInDoctorsAsync(int userId, int doctorId);

    // Check if a doctor exists
    public Task<bool> DoesDoctorExistAsync(int doctorId);
}