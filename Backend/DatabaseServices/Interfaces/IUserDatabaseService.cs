using System.Threading.Tasks;

namespace Backend.DatabaseServices.Interfaces
{
    public interface IUserDatabaseService
    {
        public Task<bool> UserExistsWithRole(int userID, string role);
        public Task<int> GetUserId(string username, string password);
    }
}