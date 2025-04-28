namespace Hospital.DatabaseServices.Interfaces
{
    public interface IUserDatabaseService
    {
        public bool UserExistsWithRole(int userID, string role);
    }
}