using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RefereeApp.Abstractions;
using RefereeApp.Models.ClubModels;

namespace RefereeApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly IClubService _clubService;

        public ClubController(IClubService clubService)
        {
            _clubService = clubService;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<ClubResponseModel>>> Get()
        {
            var club = await _clubService.Get();
            return Ok(club);
        }

        [HttpGet("id")]
        public async Task<ActionResult<ClubResponseModel>> Get([FromQuery] int id)
        {
            var club = await _clubService.GetById(id);
            return Ok(club);
        }

        [HttpPost]
        public async Task<ActionResult<ClubResponseModel>> Create(CreateClubRequestModel request)
        {
            var club = await _clubService.Create(request);
            return Ok(club);
        }

        [HttpPut]
        public async Task<ActionResult<ClubResponseModel>> Update(UpdateClubRequestModel request)
        {
            var club = await _clubService.Update(request);
            return Ok(club);
        }
        
        
    }


}