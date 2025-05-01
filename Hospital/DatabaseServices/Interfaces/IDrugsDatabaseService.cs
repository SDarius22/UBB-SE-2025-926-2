using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.DatabaseServices.Interfaces
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