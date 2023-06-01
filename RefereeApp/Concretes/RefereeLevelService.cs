using Microsoft.EntityFrameworkCore;
using RefereeApp.Abstractions;
using RefereeApp.Data;
using RefereeApp.Entities;
using RefereeApp.Models.RefLevels;

namespace RefereeApp.Concretes;

public class RefereeLevelService : IRefereeLevelService
{
    private readonly ApplicationDbContext _applicationDbContext;

    public RefereeLevelService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }


    public async Task<List<RefereeLevelsResponseModel>> Get()
    {
        var response = await _applicationDbContext.RefereeLevels
            .Select(x => new RefereeLevelsResponseModel()
            {
                Id = x.Id,
                StatusLevel = x.StatusLevel,
                IsDeleted = x.IsDeleted,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                ChangedAt = x.ChangedAt,
                ChangedBy = x.ChangedBy
            }).ToListAsync();

        if (response == default)
        {
            throw new Exception("Response model boş");
        }

        return response;
    }

    public async Task<RefereeLevelsResponseModel> Create(CreateRefereeLevelsRequestModel request)
    {
        var refereeLevel = new RefereeLevel()
        {
            StatusLevel = request.StatusLevel,
            CreatedAt = request.CreatedAt,
            CreatedBy = request.CreatedBy,
            ChangedAt = request.ChangedAt,
            ChangedBy = request.ChangedBy,
            IsDeleted = request.IsDeleted
        };

        _applicationDbContext.Add(refereeLevel);
        await _applicationDbContext.SaveChangesAsync();

        var response = new RefereeLevelsResponseModel()
        {
            Id = refereeLevel.Id,
            StatusLevel = refereeLevel.StatusLevel,
            CreatedAt = refereeLevel.CreatedAt,
            CreatedBy = refereeLevel.CreatedBy,
            ChangedAt = refereeLevel.ChangedAt,
            ChangedBy = refereeLevel.ChangedBy,
            IsDeleted = refereeLevel.IsDeleted
        };

        return response;
    }

    public async Task<RefereeLevelsResponseModel> Update(UpdateRefereeLevelsRequestModel request)
    {
        var entity = await _applicationDbContext.RefereeLevels
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();

        if (entity == default)
        {
            throw new Exception("Entity is null");
        }

        entity.StatusLevel = request.StatusLevel;
        entity.ChangedAt = request.ChangedAt;
        entity.ChangedBy = request.ChangedBy;
        entity.IsDeleted = request.IsDeleted;

        await _applicationDbContext.SaveChangesAsync();

        var response = new RefereeLevelsResponseModel()
        {
            Id = entity.Id,
            StatusLevel = entity.StatusLevel,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            ChangedAt = entity.ChangedAt,
            ChangedBy = entity.ChangedBy,
            IsDeleted = entity.IsDeleted
        };

        return response;

    }
}