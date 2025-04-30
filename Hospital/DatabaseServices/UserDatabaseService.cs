using Hospital.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public class UserDatabaseService
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
            return await _context.Users.AnyAsync(u => u.UserId == userID && u.Role == role);
        }
    }
}