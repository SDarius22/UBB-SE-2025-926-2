using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.DatabaseServices.Interfaces
{
    public interface IEquipmentDatabaseService
    {
        public bool AddEquipment(EquipmentModel equipment);
        public bool UpdateEquipment(EquipmentModel equipment);
        public bool DeleteEquipment(int equipmentID);
        public bool DoesEquipmentExist(int equipmentID);
        public Task<List<EquipmentModel>> GetEquipments();
    }
}