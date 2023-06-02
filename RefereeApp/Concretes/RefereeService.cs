using Microsoft.EntityFrameworkCore;
using RefereeApp.Abstractions;
using RefereeApp.Data;
using RefereeApp.Entities;
using RefereeApp.Models.RefereeModels;

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
                RefLevelId = x.RefLevelId,
                RefRegionId = x.RefRegionId
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
            .Where(x => x.Id == id)
            .Select(x => new RefereeResponseModel()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                RefLevelId = x.RefLevelId,
                RefRegionId = x.RefRegionId
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
            RefLevelId = request.RefLevelId,
            RefRegionId = request.RefRegionId,
            CreatedAt = request.CreatedAt,
            CreatedBy = request.CreatedBy,
            ChangedAt = request.ChangedAt,
            ChangedBy = request.ChangedBy
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
            RefLevelId = referee.RefLevelId,
            RefRegionId = referee.RefRegionId,
            CreatedAt = referee.CreatedAt,
            CreatedBy = referee.CreatedBy,
            ChangedAt = referee.ChangedAt,
            ChangedBy = referee.ChangedBy
        };

        return response;

    }

    public async Task<RefereeResponseModel> Update(UpdateRefereeRequestModel request)
    {
        var entity = await _applicationDbContext.Referees
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();

        if (entity == default)
        {
            throw new Exception("Entity is empty.");
        }

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Email = request.Email;
        entity.PhoneNumber = request.PhoneNumber;
        entity.RefLevelId = request.RefLevelId;
        entity.RefRegionId = request.RefRegionId;
        entity.ChangedAt = DateTime.Now;
        entity.ChangedBy = request.ChangedBy;

        var response = new RefereeResponseModel()
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            RefLevelId = entity.RefLevelId,
            RefRegionId = entity.RefRegionId,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            ChangedAt = entity.ChangedAt,
            ChangedBy = entity.ChangedBy
        };

        return response;

    }
}