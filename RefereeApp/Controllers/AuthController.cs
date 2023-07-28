using Microsoft.AspNetCore.Mvc;
using RefereeApp.Abstractions;
using RefereeApp.Models.AuthModels;
using RefereeApp.Models.ClubModels;

namespace RefereeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginResponseModel>> Login([FromBody] LoginModel request)
        {
            var user = await _authService.Login(request);
            return Ok(user);
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<RegisterResponseModel>> Register([FromBody] RegisterModel request)
        {
            var user = await _authService.Register(request);
            return Ok(user);
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<ActionResult<RegisterResponseModel>> RegisterAdmin([FromBody] RegisterModel request)
        {
            var user = await _authService.RegisterAdmin(request);
            return Ok(user);
        }
        
        [HttpPost]
        [Route("register-employee")]
        public async Task<ActionResult<RegisterResponseModel>> RegisterEmployee([FromBody] RegisterModel request)
        {
            var user = await _authService.RegisterEmployee(request);
            return Ok(user);
        }


    }


}