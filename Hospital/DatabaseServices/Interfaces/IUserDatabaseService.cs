using System.Threading.Tasks;

namespace Hospital.DatabaseServices.Interfaces
{
    public interface IUserDatabaseService
    {
        public Task<bool> UserExistsWithRole(int userID, string role);
    }
}