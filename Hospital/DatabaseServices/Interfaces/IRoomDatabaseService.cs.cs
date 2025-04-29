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
        public bool AddRoom(RoomModel room);

        public bool UpdateRoom(RoomModel room);

        public bool DeleteRoom(int roomID);

        public bool DoesRoomExist(int roomID);

        public bool DoesEquipmentExist(int equipmentID);

        public bool DoesDepartmentExist(int departmentID);

        public List<RoomModel>? GetRooms();
    }
}