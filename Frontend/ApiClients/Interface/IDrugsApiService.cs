using Frontend.Models;

namespace Frontend.ApiClients.Interface;

public interface IDrugsApiService
{

    // Get all drugs
    public Task<List<DrugModel>> GetDrugsAsync();

    public Task<DrugModel> GetDrugAsync(int id);

    // Add a new drug
    public Task<bool> AddDrugAsync(DrugModel drug);


    // Update an existing drug
    public Task<bool> UpdateDrugAsync(int drugId, DrugModel drug);

    // Delete a drug
    public Task<bool> DeleteDrugAsync(int drugId);

    // Check if a drug exists
    public Task<bool> DoesDrugExistAsync(int drugId);

}