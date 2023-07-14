using Microsoft.AspNetCore.Identity;

namespace RefereeApp.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public Referee Referee { get; set; }
}