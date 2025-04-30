using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using Microsoft.UI.Xaml.Controls;

namespace Hospital.DatabaseServices.Interfaces
{
    internal interface IRoomDatabaseService
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