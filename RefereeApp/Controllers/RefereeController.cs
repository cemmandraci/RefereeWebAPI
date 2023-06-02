﻿using Microsoft.AspNetCore.Mvc;
using RefereeApp.Abstractions;
using RefereeApp.Models.RefereeModels;
using RefereeApp.Models.RefLevels;

namespace RefereeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefereeController : ControllerBase
    {
        private readonly IRefereeService _refereeService;

        public RefereeController(IRefereeService refereeService)
        {
            _refereeService = refereeService;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<RefereeResponseModel>>> Get()
        {
            var referee = await _refereeService.Get();
            return Ok(referee);
        }

        [HttpGet("id")]
        public async Task<ActionResult<RefereeResponseModel>> Get([FromQuery] int id)
        {
            var referee = await _refereeService.GetById(id);
            return Ok(referee);
        }

        [HttpPost]
        public async Task<ActionResult<RefereeResponseModel>> Create(CreateRefereeRequestModel request)
        {
            var referee = await _refereeService.Create(request);
            return Ok(referee);
        }

        [HttpPut]
        public async Task<ActionResult<RefereeResponseModel>> Update(UpdateRefereeRequestModel request)
        {
            var referee = await _refereeService.Update(request);
            return Ok(referee);
        }
        
    }


}