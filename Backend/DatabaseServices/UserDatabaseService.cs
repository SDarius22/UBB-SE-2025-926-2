using Backend.DatabaseServices.Interfaces;
using Backend.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Backend.DatabaseServices
{
    public class UserDatabaseService : IUserDatabaseService
    {
        private readonly AppDbContext _context;

        public UserDatabaseService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userID">The id of the user.</param>
        /// <param name="role">The role of.</param>
        /// <returns>The joined names.</returns>
        public async Task<bool> UserExistsWithRole(int userID, string role)
        {
            return await _context.Users.AnyAsync(u => u.UserID == userID && u.Role == role);
        }
    }
}