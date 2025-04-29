using System.Collections.Generic;
using Hospital.Models;

namespace Hospital.DatabaseServices.Interfaces
{
    public interface IEquipmentDatabaseService
    {
        public bool AddEquipment(EquipmentModel equipment);
        public bool UpdateEquipment(EquipmentModel equipment);
        public bool DeleteEquipment(int equipmentID);
        public bool DoesEquipmentExist(int equipmentID);
        public List<EquipmentModel> GetEquipments();
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> dbf29defeda16dddeaad4741dab9f7dd8d0a692e
