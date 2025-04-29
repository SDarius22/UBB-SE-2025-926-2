namespace Hospital.DatabaseServices
{
    using System.Collections.Generic;
    using Hospital.Models;
    public interface IDrugsDatabaseService
    {
        public bool AddDrug(DrugModel drug);
        public bool UpdateDrug(DrugModel drug);
        public bool DeleteDrug(int drugID);
        public bool DoesDrugExist(int drugID);
        public List<DrugModel> GetDrugs();
    }
}
