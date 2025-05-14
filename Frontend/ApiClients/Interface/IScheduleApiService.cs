using Frontend.Models;

namespace Frontend.ApiClients.Interface;

public interface IScheduleApiService
{
    public Task<List<ScheduleModel>> GetSchedulesAsync();
    
    public Task<ScheduleModel> GetScheduleAsync(int id);

    public Task<bool> AddScheduleAsync(ScheduleModel schedule);

    public Task<bool> UpdateScheduleAsync(int scheduleId, ScheduleModel schedule);

    public Task<bool> DeleteScheduleAsync(int scheduleId);

    public Task<bool> DoesScheduleExistAsync(int scheduleId);

    public Task<bool> DoesDoctorExistAsync(int doctorId);

    public Task<bool> DoesShiftExistAsync(int shiftId);
}