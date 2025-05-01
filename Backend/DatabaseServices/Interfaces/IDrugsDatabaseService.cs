using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.DatabaseServices.Interfaces
{
    public interface IDrugsDatabaseService
    {
        public Task<bool> AddDrug(DrugModel drug);
        public Task<bool> UpdateDrug(DrugModel drug);
        public Task<bool> DeleteDrug(int drugID);
        public Task<bool> DoesDrugExist(int drugID);
        public Task<List<DrugModel>> GetDrugs();
    }
}