#nullable enable
namespace RefereeApp.Models.AuthModels;

public class LoginResponseModel
{
    public int? Status { get; set; }
    public string? Message { get; set; }
    public string token { get; set; }
    public DateTime expiration { get; set; }
}