namespace ProjectManagementAPIs.Interfaces
{
    public interface IRefreshTokenGenerator
    {
        Task<string> GenerateToken(string memberUserEmail);
    }
}
