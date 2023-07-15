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
    private readonly ILogger<AuthService> _logger;

    public AuthService(IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, ILogger<AuthService> logger)
    {
        _configuration = configuration;
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    //TODO: Loglarda register admin alanı boş, oraya bakılacak ! 
    public async Task<ResponseModel> Login(LoginModel request)
    {
        _logger.LogInformation("Login User | Function is starting.");
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
            
            _logger.LogInformation("Login User | {username} user is authenticating the system.", user.UserName);

            return new ResponseModel()
            {
                Status = 200,
                Message = "Successfull",
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            };
        }
        
        _logger.LogInformation("Login User | Login failed.");

        return new ResponseModel()
        {
            Status = 400,
            Message = "Something wrong."
        };
    }

    public async Task<ResponseModel> Register(RegisterModel request)
    {
        _logger.LogInformation("Register User | Function is starting");
        var userExist = await _userManager.FindByNameAsync(request.UserName);
        if (userExist != null)
        {
            _logger.LogInformation("Register User | {username} does not exist.",request.UserName);
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
            _logger.LogInformation("Register User | New user couldn't be created.");
            return new ResponseModel() { Status = 400, Message = "Something wrong." };
        }

        var response = new ResponseModel()
        {
            Status = 200,
            Message = "User has been succesfully created."
        };
        
        _logger.LogInformation("Register User | User successfully created.");

        return response;

    }
    
    
    //TODO : Register admin ile roller nasıl çalışıyor , debug atılacak , belki güncellenebilir !
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

    //TODO : Token süresi uzatılabilir.
    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddDays(1),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
    
}