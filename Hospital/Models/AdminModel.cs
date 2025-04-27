using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class AdminModel
    {
        public int AdminId { get; set; }
        public int UserId { get; set; }

        public AdminModel(int adminId, int userId)
        {
            AdminId = adminId;
            UserId = userId;
        }
    }
}
