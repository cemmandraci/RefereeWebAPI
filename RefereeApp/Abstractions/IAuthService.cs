using RefereeApp.Models.AuthModels;

namespace RefereeApp.Abstractions;

public interface IAuthService
{
    public Task<ResponseModel> Login(LoginModel request);
    public Task<ResponseModel> Register(RegisterModel request);
    public Task<ResponseModel> RegisterAdmin(RegisterModel request);
    string GetUserIdFromToken();
}