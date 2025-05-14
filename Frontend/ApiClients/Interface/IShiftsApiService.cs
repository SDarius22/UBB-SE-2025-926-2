using Frontend.Models;

namespace Frontend.ApiClients.Interface;

public interface IShiftsApiService
{
    public Task<List<ShiftModel>> GetShiftsAsync();

    public Task<ShiftModel> GetShiftAsync(int id);

    public Task<List<ShiftModel>> GetShiftsByDoctorIdAsync(int doctorId);

    public Task<List<ShiftModel>> GetDoctorDaytimeShiftsAsync(int doctorId);

    public Task<bool> AddShiftAsync(ShiftModel shift);

    public Task<bool> UpdateShiftAsync(int shiftId, ShiftModel shift);

    public Task<bool> DeleteShiftAsync(int shiftId);

    public Task<bool> DoesShiftExistAsync(int shiftId);
}