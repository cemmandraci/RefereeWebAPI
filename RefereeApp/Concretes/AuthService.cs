using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

    //TODO: Loglarda register admin alanı boş, oraya bakılacak ! 
    //TODO: Log testleri çevrilecek !
    public async Task<ResponseModel> Login(LoginModel request)
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

            var token = GetToken(authClaims);
            
            _logger.LogInformation("Login User | {username} user is authenticating the system.", user.UserName);

            return new ResponseModel()
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            };
        }
        
        _logger.LogError("Login User | Login failed.");
        throw new BadRequestException("Username or password is not correct !");
    }

    public async Task<ResponseModel> Register(RegisterModel request)
    {
        _logger.LogInformation("Register User | Function is starting");
        var userExist = await _userManager.FindByNameAsync(request.UserName);
        if (userExist != null)
        {
            _logger.LogInformation("Register User | {username} does not exist.",request.UserName);
            throw new NotFoundException("An error occured !");
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
            throw new BadRequestException("An error occured !");
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
            throw new BadRequestException("An error occured !");
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

    public string GetUserIdFromToken()
    {

        if (_httpContextAccessor.HttpContext != null)
        {
            string userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            return userId;
        }

        throw new BadRequestException("An error occured");
    }

    //TODO : Token süresi uzatılabilir.//DONE
    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? throw new NullReferenceException("An error occured !")));

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