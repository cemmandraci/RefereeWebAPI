using Microsoft.EntityFrameworkCore;
using RefereeApp.Abstractions;
using RefereeApp.Data;
using RefereeApp.Entities;
using RefereeApp.Entities.Enums;
using RefereeApp.Exceptions;
using RefereeApp.Models.RefereeModels;
using RefereeApp.Models.RefereeModels.RefereeMatches;
using RefereeApp.Models.RefereeModels.RefereeRegions;
using RefereeApp.Models.RefLevels;

namespace RefereeApp.Concretes;

public class RefereeService : IRefereeService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<RefereeService> _logger;
    private readonly IAuthService _authService;

    public RefereeService(ApplicationDbContext applicationDbContext, ILogger<RefereeService> logger, IAuthService authService)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
        _authService = authService;
    }

    public async Task<List<RefereeResponseModel>> Get()
    {
        _logger.LogInformation("Referee Get() | Function is starting.");
        var response = await _applicationDbContext.Referees
            .AsNoTracking()
            .Select(x => new RefereeResponseModel()
            {
                Id = x.Id,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                ChangedAt = x.ChangedAt,
                ChangedBy = x.ChangedBy,
                IsDeleted = x.IsDeleted,
                RefereeLevel = new RefereeLevelsResponseModel()
                {
                    Id = x.RefereeLevel.Id,
                    StatusLevel = x.RefereeLevel.StatusLevel,
                    CreatedAt = x.RefereeLevel.CreatedAt,
                    CreatedBy = x.RefereeLevel.CreatedBy,
                    ChangedAt = x.RefereeLevel.ChangedAt,
                    ChangedBy = x.RefereeLevel.ChangedBy,
                    IsDeleted = x.RefereeLevel.IsDeleted
                },
                RefereeRegion = new RefereeRegionResponseModel()
                {
                    Id = x.RefereeRegion.Id,
                    RegionId = x.RefereeRegion.RegionId,
                    CreatedAt = x.RefereeRegion.CreatedAt,
                    CreatedBy = x.RefereeRegion.CreatedBy,
                    ChangedAt = x.RefereeRegion.ChangedAt,
                    ChangedBy = x.RefereeRegion.ChangedBy,
                    IsDeleted = x.RefereeRegion.IsDeleted
                }
            }).ToListAsync();

        if (response == default)
        {
            _logger.LogError("Referee Get() | There isn't exist referee list, fetch is failed.");
            throw new NotFoundException("An error occured !");
        }

        _logger.LogInformation("Referee Get() | Referee list fetched successfully.");
        return response;
    }

    public async Task<RefereeResponseModel> GetById(int id)
    {
        _logger.LogInformation("Referee GetById() | Function is starting.");
        var response = await _applicationDbContext.Referees
            .AsNoTracking()
            .Include(x=>x.RefereeLevel)
            .Include(x=>x.RefereeRegion)
            .Where(x => x.Id == id)
            .Select(x => new RefereeResponseModel()
            {
                Id = x.Id,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                ChangedAt = x.ChangedAt,
                ChangedBy = x.ChangedBy,
                IsDeleted = x.IsDeleted,
                RefereeLevel = new RefereeLevelsResponseModel()
                {
                    Id = x.RefereeLevel.Id,
                    StatusLevel = x.RefereeLevel.StatusLevel,
                    CreatedAt = x.RefereeLevel.CreatedAt,
                    CreatedBy = x.RefereeLevel.CreatedBy,
                    ChangedAt = x.RefereeLevel.ChangedAt,
                    ChangedBy = x.RefereeLevel.ChangedBy,
                    IsDeleted = x.RefereeLevel.IsDeleted
                },
                RefereeRegion = new RefereeRegionResponseModel()
                {
                    Id = x.RefereeRegion.Id,
                    RegionId = x.RefereeRegion.RegionId,
                    CreatedAt = x.RefereeRegion.CreatedAt,
                    CreatedBy = x.RefereeRegion.CreatedBy,
                    ChangedAt = x.RefereeRegion.ChangedAt,
                    ChangedBy = x.RefereeRegion.ChangedBy,
                    IsDeleted = x.RefereeRegion.IsDeleted
                }
            }).FirstOrDefaultAsync();

        if (response == default)
        {
            _logger.LogError("Referee GetById() | There is no any {id} id' referee entity, fetch is failed.", id);
            throw new NotFoundException("An error occured !");
        }
        
        _logger.LogInformation("Referee GetById() | Fetch is successful.");
        return response;
    }
    
    public async Task<RefereeResponseModel> Create(CreateRefereeRequestModel request)
    {
        var username = _authService.GetUsernameFromToken();
        _logger.LogInformation("Referee Create() | Function is starting.");
        var referee = new Referee()
        {
            UserId = request.UserId,
            IsActive = request.IsActive,
            CreatedAt = DateTime.Now,
            CreatedBy = username,
            ChangedAt = DateTime.Now,
            ChangedBy = username,
            LastAttendMatch = DateTime.Now,
            RefereeRegion = new RefereeRegion()
            {
                RegionId = request.RefereeRegion.RegionId,
                CreatedAt = DateTime.Now,
                CreatedBy = username,
                ChangedAt = DateTime.Now,
                ChangedBy = username,
                IsDeleted = request.RefereeRegion.IsDeleted
            },
            RefereeLevel = new RefereeLevel()
            {
                StatusLevel = request.RefereeLevels.StatusLevel,
                CreatedAt = DateTime.Now,
                CreatedBy = username,
                ChangedAt = DateTime.Now,
                ChangedBy = username,
                IsDeleted = request.RefereeLevels.IsDeleted
            },
            RefereeMatch = request.RefereeMatch.Select(x=> new RefereeMatch()
            {
                LastAttendMatch = DateTime.Now,
                CreatedAt = DateTime.Now,
                CreatedBy = username,
                ChangedAt = DateTime.Now,
                ChangedBy = username,
                IsDeleted = x.IsDeleted
            }).ToList()
        };
        

        _applicationDbContext.Add(referee);
        await _applicationDbContext.SaveChangesAsync();

        var response = new RefereeResponseModel()
        {
            Id = referee.Id,
            IsActive = referee.IsActive,
            UserId = referee.UserId,
            LastAttendMatch = referee.LastAttendMatch,
            CreatedAt = referee.CreatedAt,
            CreatedBy = referee.CreatedBy,
            ChangedAt = referee.ChangedAt,
            ChangedBy = referee.ChangedBy,
            RefereeRegion = new RefereeRegionResponseModel()
            {
                Id = referee.RefereeRegion.Id,
                RegionId = referee.RefereeRegion.RegionId,
                CreatedAt = referee.RefereeRegion.CreatedAt,
                CreatedBy = referee.RefereeRegion.CreatedBy,
                ChangedAt = referee.RefereeRegion.ChangedAt,
                ChangedBy = referee.RefereeRegion.ChangedBy,
                IsDeleted = referee.RefereeRegion.IsDeleted
            },
            RefereeLevel = new RefereeLevelsResponseModel()
            {
                Id = referee.RefereeLevel.Id,
                StatusLevel = referee.RefereeLevel.StatusLevel,
                CreatedAt = referee.RefereeLevel.CreatedAt,
                CreatedBy = referee.RefereeLevel.CreatedBy,
                ChangedAt = referee.RefereeLevel.ChangedAt,
                ChangedBy = referee.RefereeLevel.ChangedBy,
                IsDeleted = referee.RefereeLevel.IsDeleted
            },
            RefereeMatch = referee.RefereeMatch.Select(match=> new RefereeMatchResponseModel()
            {
                Id = match.Id,
                RefereeId = match.RefereeId,
                LastAttendMatch = match.LastAttendMatch,
                CreatedAt = match.CreatedAt,
                CreatedBy = match.CreatedBy,
                ChangedAt = match.ChangedAt,
                ChangedBy = match.ChangedBy
            }).ToList()
        };

        _logger.LogInformation("Referee Create() | Referee entity is created successfully.");
        
        return response;

    }
    
    public async Task<RefereeResponseModel> Update(UpdateRefereeRequestModel request)
    {
        var username = _authService.GetUsernameFromToken();
        _logger.LogInformation("Referee Update() | Function is starting.");
        _logger.LogInformation("Referee Update() | To finding entity from db is starting.");
        var entity = await _applicationDbContext.Referees
            .Include(x=>x.RefereeLevel)
            .Include(x=>x.RefereeRegion)
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();

        if (entity == default)
        {
            _logger.LogError("Referee Update() | Fetching {id} entity is failed.", request.Id);
            throw new NotFoundException("An error occured !");
        }

        if (request.UserId is not null) entity.UserId = request.UserId;
        if (request.IsActive is not null) entity.IsActive = (bool)request.IsActive;
        if (request.RefereeRegion.RegionId is not null) entity.RefereeRegion.RegionId = (Region)request.RefereeRegion.RegionId;
        if (request.RefereeRegion.IsDeleted is not null) entity.RefereeRegion.IsDeleted = (bool)request.RefereeRegion.IsDeleted;
        if (request.RefereeLevel.StatusLevel is not null) entity.RefereeLevel.StatusLevel = request.RefereeLevel.StatusLevel;
        if (request.RefereeLevel.IsDeleted is not null) entity.RefereeLevel.IsDeleted = (bool)request.RefereeLevel.IsDeleted;
        entity.IsDeleted = request.IsDeleted;
        entity.ChangedBy = username;
        entity.ChangedAt = DateTime.Now;
        entity.RefereeRegion.ChangedAt = DateTime.Now;
        entity.RefereeRegion.ChangedBy = username;
        entity.RefereeLevel.ChangedAt = DateTime.Now;
        entity.RefereeLevel.ChangedBy = username;

        await _applicationDbContext.SaveChangesAsync();

        var response = new RefereeResponseModel()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            IsActive = entity.IsActive,
            LastAttendMatch = entity.LastAttendMatch,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            ChangedAt = entity.ChangedAt,
            ChangedBy = entity.ChangedBy,
            IsDeleted = entity.IsDeleted,
            RefereeLevel = new RefereeLevelsResponseModel()
            {
                Id = entity.RefereeLevel.Id,
                StatusLevel = entity.RefereeLevel.StatusLevel,
                CreatedAt = entity.RefereeLevel.CreatedAt,
                CreatedBy = entity.RefereeLevel.CreatedBy,
                ChangedAt = entity.RefereeLevel.ChangedAt,
                ChangedBy = entity.RefereeLevel.ChangedBy,
                IsDeleted = entity.RefereeLevel.IsDeleted
            },
            RefereeRegion = new RefereeRegionResponseModel()
            {
                Id = entity.RefereeRegion.Id,
                RegionId = entity.RefereeRegion.RegionId,
                CreatedAt = entity.RefereeRegion.CreatedAt,
                CreatedBy = entity.RefereeRegion.CreatedBy,
                ChangedAt = entity.RefereeRegion.ChangedAt,
                ChangedBy = entity.RefereeRegion.ChangedBy,
                IsDeleted = entity.RefereeRegion.IsDeleted
            }
        };
        
        _logger.LogInformation("Referee Update() | Referee entity is updated successfully.");
        
        return response;

    }
}