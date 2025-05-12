using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class AdminModel
    {
        // Primary key
        public int AdminId { get; set; }

        // Foreign key to User
        public int UserId { get; set; }

        // Navigation property (optional but recommended)
        public virtual UserModel? User { get; set; }

        // Parameterless constructor required by EF Core
        public AdminModel() { }

        // Convenience constructor for application code
        public AdminModel(int adminId, int userId)
        {
            AdminId = adminId;
            UserId = userId;
        }
    }
}
