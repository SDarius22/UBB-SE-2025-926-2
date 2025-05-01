using Backend.Models;

namespace Backend.DatabaseServices.Interfaces
{
    public interface IRoomDatabaseService
    {
        public Task<bool> AddRoom(RoomModel room);

        public Task<bool> UpdateRoom(RoomModel room);

        public Task<bool> DeleteRoom(int roomID);

        public Task<bool> DoesRoomExist(int roomID);

        public Task<bool> DoesEquipmentExist(int equipmentID);

        public Task<bool> DoesDepartmentExist(int departmentID);

        public Task<List<RoomModel>?> GetRooms();
    }
}