using Frontend.Models;

namespace Frontend.ApiClients.Interface;

public interface IDepartmentsApiService
{
    // Get all departments
    public Task<List<DepartmentModel>> GetDepartmentsAsync();

    public Task<DepartmentModel> GetDepartmentAsync(int departmentId);

    // Add a new department
    public Task<bool> AddDepartmentAsync(DepartmentModel department);

    // Update an existing department
    public Task<bool> UpdateDepartmentAsync(int departmentId, DepartmentModel department);

    // Delete a department
    public Task<bool> DeleteDepartmentAsync(int departmentId);

    // Check if a department exists
    public Task<bool> DoesDepartmentExistAsync(int departmentId);
}