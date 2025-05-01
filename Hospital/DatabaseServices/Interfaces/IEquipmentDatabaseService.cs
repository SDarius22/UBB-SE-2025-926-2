using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.DatabaseServices.Interfaces
{
    public interface IEquipmentDatabaseService
    {
        public Task<bool> AddEquipment(EquipmentModel equipment);
        public Task<bool> UpdateEquipment(EquipmentModel equipment);
        public Task<bool> DeleteEquipment(int equipmentID);
        public Task<bool> DoesEquipmentExist(int equipmentID);
        public Task<List<EquipmentModel>> GetEquipments();
    }
}