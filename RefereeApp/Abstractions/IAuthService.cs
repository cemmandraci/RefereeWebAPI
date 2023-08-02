using RefereeApp.Models.AuthModels;

namespace RefereeApp.Abstractions;

public interface IAuthService
{
    public Task<LoginResponseModel> Login(LoginModel request);
    public Task<RegisterResponseModel> Register(RegisterModel request);
    public Task<RegisterResponseModel> RegisterAdmin(RegisterModel request);
    public Task<RegisterResponseModel> RegisterEmployee(RegisterModel request);
    string GetUsernameFromToken();
    string GetUserIdFromToken();
}