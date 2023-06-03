using Microsoft.EntityFrameworkCore;
using RefereeApp.Abstractions;
using RefereeApp.Data;
using RefereeApp.Entities;
using RefereeApp.Entities.Enums;
using RefereeApp.Models.RefereeModels;
using RefereeApp.Models.RefLevels;

namespace RefereeApp.Concretes;

public class RefereeService : IRefereeService
{
    private readonly ApplicationDbContext _applicationDbContext;

    public RefereeService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<List<RefereeResponseModel>> Get()
    {
        var response = await _applicationDbContext.Referees
            .AsNoTracking()
            .Select(x => new RefereeResponseModel()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
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
            throw new Exception("Entity can not be empty.");
        }

        return response;
    }

    public async Task<RefereeResponseModel> GetById(int id)
    {
        var response = await _applicationDbContext.Referees
            .AsNoTracking()
            .Include(x=>x.RefereeLevel)
            .Include(x=>x.RefereeRegion)
            .Where(x => x.Id == id)
            .Select(x => new RefereeResponseModel()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
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
            throw new Exception("Entity can not be empty");
        }

        return response;
    }

    public async Task<RefereeResponseModel> Create(CreateRefereeRequestModel request)
    {
        var referee = new Referee()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsActive = request.IsActive,
            CreatedAt = request.CreatedAt,
            CreatedBy = request.CreatedBy,
            ChangedAt = request.ChangedAt,
            ChangedBy = request.ChangedBy,
            RefereeRegion = new RefereeRegion()
            {
                RegionId = request.RefereeRegion.RegionId,
                CreatedAt = request.RefereeRegion.CreatedAt,
                CreatedBy = request.RefereeRegion.CreatedBy,
                ChangedAt = request.RefereeRegion.ChangedAt,
                ChangedBy = request.RefereeRegion.ChangedBy,
                IsDeleted = request.RefereeRegion.IsDeleted
            },
            RefereeLevel = new RefereeLevel()
            {
                StatusLevel = request.RefereeLevels.StatusLevel,
                CreatedAt = request.RefereeLevels.CreatedAt,
                CreatedBy = request.RefereeLevels.CreatedBy,
                ChangedAt = request.RefereeLevels.ChangedAt,
                ChangedBy = request.RefereeLevels.ChangedBy,
                IsDeleted = request.RefereeLevels.IsDeleted
            }
        };

        _applicationDbContext.Add(referee);
        await _applicationDbContext.SaveChangesAsync();

        var response = new RefereeResponseModel()
        {
            Id = referee.Id,
            FirstName = referee.FirstName,
            LastName = referee.LastName,
            Email = referee.Email,
            PhoneNumber = referee.PhoneNumber,
            IsActive = referee.IsActive,
            RefLevelId = referee.RefLevelId,
            RefRegionId = referee.RefRegionId,
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
            }
        };

        return response;

    }

    public async Task<RefereeResponseModel> Update(UpdateRefereeRequestModel request)
    {
        var entity = await _applicationDbContext.Referees
            .Include(x=>x.RefereeLevel)
            .Include(x=>x.RefereeRegion)
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();

        if (entity == default)
        {
            throw new Exception("Entity is empty.");
        }

        if (request.FirstName is not null) entity.FirstName = request.FirstName;
        if (request.LastName is not null) entity.LastName = request.LastName;
        if (request.Email is not null) entity.Email = request.Email;
        if(request.IsActive is not null) entity.IsActive = (bool)request.IsActive;
        entity.PhoneNumber = request.PhoneNumber;
        entity.ChangedBy = request.ChangedBy;
        if (request.RefereeLevel.StatusLevel is not null) entity.RefereeLevel.StatusLevel = (int)request.RefereeLevel.StatusLevel;
        entity.RefereeLevel.ChangedBy = request.RefereeLevel.ChangedBy;
        if (request.RefereeRegion.RegionId is not null) entity.RefereeRegion.RegionId = (Region)request.RefereeRegion.RegionId;
        entity.RefereeRegion.ChangedBy = request.RefereeRegion.ChangedBy;

        var response = new RefereeResponseModel()
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            RefLevelId = entity.RefLevelId,
            RefRegionId = entity.RefRegionId,
            IsActive = entity.IsActive,
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

        return response;

    }
}