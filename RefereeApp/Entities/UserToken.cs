using Microsoft.AspNetCore.Identity;

namespace RefereeApp.Entities;

public class UserToken : IdentityUserToken<string>
{
    public DateTime ExpireDate { get; set; }
}