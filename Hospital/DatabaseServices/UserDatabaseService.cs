using Hospital.Configs;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserModel = Hospital.Models.UserModel;

namespace Hospital.DatabaseServices
{
    public class UserDatabaseService
    {
        private readonly ApplicationConfiguration _configuration;

        public UserDatabaseService()
        {
            _configuration = ApplicationConfiguration.GetInstance();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userID">The id of the user.</param>
        /// <param name="role">The role of.</param>
        /// <returns>The joined names.</returns>
        public bool UserExistsWithRole(int userID, string role)
        {
            using (SqlConnection connection = new SqlConnection(this._configuration.DatabaseConnection))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE UserID = @UserID AND Role = @Role";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@Role", role);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }
    }
}
