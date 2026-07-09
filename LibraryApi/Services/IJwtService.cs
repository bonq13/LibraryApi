namespace LibraryApi.Services;

public interface IJwtService
{
    public string GenerateToken(string userId, string email, string role);
}