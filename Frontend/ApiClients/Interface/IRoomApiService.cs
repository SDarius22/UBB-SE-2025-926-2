using Frontend.Models;

namespace Frontend.ApiClients.Interface;

public interface IRoomApiService
{
    public Task<List<RoomModel>> GetRoomsAsync();

    public Task<RoomModel> GetRoomAsync(int id);

    public Task<bool> AddRoomAsync(RoomModel room);

    public Task<bool> UpdateRoomAsync(int roomId, RoomModel room);

    public Task<bool> DeleteRoomAsync(int roomId);

    // Check if a room exists
    public Task<bool> DoesRoomExistAsync(int roomId);

    // Check if equipment exists
    public Task<bool> DoesEquipmentExistAsync(int equipmentId);

    // Check if department exists
    public Task<bool> DoesDepartmentExistAsync(int departmentId);
}