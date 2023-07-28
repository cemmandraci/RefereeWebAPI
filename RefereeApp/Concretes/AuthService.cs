using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.Pkcs;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RefereeApp.Abstractions;
using RefereeApp.Entities;
using RefereeApp.Exceptions;
using RefereeApp.Models.AuthModels;
using NotImplementedException = System.NotImplementedException;

namespace RefereeApp.Concretes;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    //TODO: Loglarda register admin alanı boş, oraya bakılacak ! //DONE
    //TODO: Log testleri çevrilecek ! //DONE
    public async Task<LoginResponseModel> Login(LoginModel request)
    {
        _logger.LogInformation("Login User | Function is starting.");
        var user = await _userManager.FindByNameAsync(request.UserName);

        if (user != default && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? throw new NotFoundException("An error occured !")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GetToken(authClaims,user.Id);
            
            _logger.LogInformation("Login User | {username} user is authenticating the system.", user.UserName);

            return new LoginResponseModel()
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            };
        }
        
        _logger.LogError("Login User | Login failed.");
        throw new BadRequestException("Username or password is not correct !");
    }

    public async Task<RegisterResponseModel> Register(RegisterModel request)
    {
        _logger.LogInformation("RegisterUser() | Function is starting");
        var userExist = await _userManager.FindByNameAsync(request.UserName);
        if (userExist != null)
        {
            _logger.LogInformation("Register() | {username} does not exist.",request.UserName);
            throw new NotFoundException("An error occured !");
        }
        
        _logger.LogInformation("Register() | User creation is starting");

        var newUser = new User()
        {
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.UserName
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            _logger.LogInformation("Register() | New user couldn't be created.");
            throw new BadRequestException("An error occured !");
        }
        
        _logger.LogInformation("Register() | Default role assign is starting.");

        var defaultRole = _roleManager.FindByNameAsync(UserRoles.Referee).Result;

        if (defaultRole != null)
        {
            if (defaultRole.Name != null)
            {
                IdentityResult roleResult = await _userManager.AddToRoleAsync(newUser, defaultRole.Name);
            }
        }

        var response = new RegisterResponseModel()
        {
            Status = 200,
            Message = "User has been succesfully created."
        };
        
        _logger.LogInformation("Register() | User has been successfully created.");

        return response;

    }
    
    
    //TODO : Register admin ile roller nasıl çalışıyor , debug atılacak , belki güncellenebilir ! //DONE
    //TODO : Loglar kontrol edilecek !
    public async Task<RegisterResponseModel> RegisterAdmin(RegisterModel request)
    {
        _logger.LogInformation("RegisterAdmin() | Function is starting");
        _logger.LogInformation("RegisterAdmin() | Exist user controlling");
        var userExist = await _userManager.FindByNameAsync(request.UserName);
        if (userExist != default)
        {
            _logger.LogError("{username} username is already exist in database !",request.UserName);
            throw new BadRequestException("An error occured !");
        }
        
        _logger.LogInformation("RegisterAdmin() | User creation is starting");
        
        var user = new User()
        {
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.UserName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            _logger.LogError("RegisterAdmin() | CreateAsync() couldn't created.");
            throw new BadRequestException("An error occured !");
        }
        
        _logger.LogInformation("RegisterAdmin() | Role assigns are starting.");

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
        
        _logger.LogInformation("RegisterAdmin() | User has been successfully created.");
        
        return new RegisterResponseModel() { Status = 200, Message = "User has been successfully created." };

    }

    public async Task<RegisterResponseModel> RegisterEmployee(RegisterModel request)
    {
        _logger.LogInformation("RegisterEmployee() | Function is starting");
        _logger.LogInformation("RegisterEmployee() | Exist user controlling");
        var userExist = await _userManager.FindByNameAsync(request.UserName);
        if (userExist != null)
        {
            _logger.LogError("RegisterEmployee() | {username} username has already exist in database",request.UserName);
            throw new BadRequestException("An error occured !");
        }
        
        _logger.LogInformation("RegisterEmployee() | User creation is starting");

        var user = new User()
        {
            UserName = request.Email,
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            _logger.LogError("RegisterEmployee() | CreateAsync() couldn't created.");
            throw new BadRequestException("An error occured !");
        }
        
        if (await _roleManager.RoleExistsAsync(UserRoles.Employee))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Employee);
        }
        
        if (await _roleManager.RoleExistsAsync(UserRoles.Referee))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Referee);
        }
        
        _logger.LogInformation("RegisterEmployee() | User has been successfully created.");

        return new RegisterResponseModel()
        {
            Status = 200,
            Message = "User has been successfully created."
        };

    }

    public string GetUsernameFromToken()
    {

        if (_httpContextAccessor.HttpContext != null)
        {
            string username = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            return username;
        }

        throw new BadRequestException("An error occured");
    }

    public string GetUserIdFromToken()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            string userId = _httpContextAccessor.HttpContext.User.Claims.First(i => i.Type == ClaimTypes.NameIdentifier)
                .Value;

            return userId;
        }

        throw new BadRequestException("An error occured!");
    }

    //TODO : Token süresi uzatılabilir.//DONE
    private JwtSecurityToken GetToken(List<Claim> authClaims, string userId)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? throw new NullReferenceException("An error occured !")));
        
        authClaims.Add(new Claim("sub", userId.ToString()));
        
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