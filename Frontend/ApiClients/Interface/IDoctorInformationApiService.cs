using Frontend.Models;

namespace Frontend.ApiClients.Interface;

public interface IDoctorInformationApiService
{
    public Task<DoctorInformationModel> GetDoctorInformationAsync(int doctorId);
}