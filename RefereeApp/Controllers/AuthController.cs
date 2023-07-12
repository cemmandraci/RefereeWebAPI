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
        public async Task<ActionResult<ResponseModel>> Login([FromBody] LoginModel request)
        {
            var user = await _authService.Login(request);
            return Ok(user);
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<ResponseModel>> Register([FromBody] RegisterModel request)
        {
            var user = await _authService.Register(request);
            return Ok(user);
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<ActionResult<ResponseModel>> RegisterAdmin([FromBody] RegisterModel request)
        {
            var user = await _authService.RegisterAdmin(request);
            return Ok(user);
        }


    }


}