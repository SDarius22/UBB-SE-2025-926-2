using Frontend.Models;

namespace Frontend.ApiClients.Interface;

public interface IMedicalProceduresApiService
{
    public Task<List<ProcedureModel>> GetProceduresByDepartmentAsync(int departmentId);
}