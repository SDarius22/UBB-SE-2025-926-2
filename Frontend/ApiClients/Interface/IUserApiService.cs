namespace Frontend.ApiClients.Interface;

public interface IUserApiService
{
    public Task<bool> CheckUserRoleAsync(int userId, string role);
}