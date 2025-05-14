using Frontend.Models;

namespace Frontend.ApiClients.Interface;

public interface IEquipmentApiService
{
    // Get all equipment
    public Task<List<EquipmentModel>> GetEquipmentsAsync();

    public Task<EquipmentModel> GetEquipmentAsync(int id);

    // Add a new equipment
    public Task<bool> AddEquipmentAsync(EquipmentModel equipment);

    // Update an existing equipment
    public Task<bool> UpdateEquipmentAsync(int equipmentId, EquipmentModel equipment);

    // Delete equipment
    public Task<bool> DeleteEquipmentAsync(int equipmentId);

    // Check if equipment exists
    public Task<bool> DoesEquipmentExistAsync(int equipmentId);
}