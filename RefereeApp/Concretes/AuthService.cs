using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RefereeApp.Abstractions;
using RefereeApp.Entities;
using RefereeApp.Models.AuthModels;

namespace RefereeApp.Concretes;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
    {
        _configuration = configuration;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<ResponseModel> Login(LoginModel request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);

        if (user != default && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GetToken(authClaims);

            return new ResponseModel()
            {
                Status = 200,
                Message = "Successfull",
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            };
        }

        return new ResponseModel()
        {
            Status = 400,
            Message = "Something wrong."
        };
    }

    public async Task<ResponseModel> Register(RegisterModel request)
    {
        var userExist = await _userManager.FindByNameAsync(request.UserName);
        if (userExist != null)
        {
            return new ResponseModel() { Status = 500, Message = "Hata"};
        }

        var newUser = new User()
        {
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.UserName
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            return new ResponseModel() { Status = 400, Message = "Something wrong." };
        }

        var response = new ResponseModel()
        {
            Status = 200,
            Message = "User has been succesfully created."
        };

        return response;

    }

    public async Task<ResponseModel> RegisterAdmin(RegisterModel request)
    {
        var userExist = await _userManager.FindByNameAsync(request.UserName);
        if (userExist != default)
        {
            return new ResponseModel() { Status = 400, Message = "Username is already exist." };
        }

        var user = new User()
        {
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.UserName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new ResponseModel() { Status = 500, Message = "User creation failed!" };
        }

        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        }

        if (!await _roleManager.RoleExistsAsync(UserRoles.Employee))
        {
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Employee));
        }
        
        if (!await _roleManager.RoleExistsAsync(UserRoles.Referee))
        {
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Referee));
        }

        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        }
        
        if (await _roleManager.RoleExistsAsync(UserRoles.Employee))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Employee);
        }
        
        if (await _roleManager.RoleExistsAsync(UserRoles.Referee))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Referee);
        }

        return new ResponseModel() { Status = 200, Message = "User created successfully" };

    }


    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
    
}