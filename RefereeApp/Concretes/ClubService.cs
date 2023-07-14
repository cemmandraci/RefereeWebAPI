using Microsoft.EntityFrameworkCore;
using RefereeApp.Abstractions;
using RefereeApp.Data;
using RefereeApp.Entities;
using RefereeApp.Models.ClubModels;

namespace RefereeApp.Concretes;

public class ClubService : IClubService
{
    private readonly ApplicationDbContext _applicationDbContext;

    public ClubService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    //TODO : Club - Genel test çevrilecek.
    public async Task<ClubResponseModel> Get(int id)
    {
        var response = await _applicationDbContext.Clubs
            .Where(x => x.Id == id)
            .Select(x => new ClubResponseModel()
            {
                Id = x.Id,
                ClubName = x.ClubName,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                ChangedAt = x.ChangedAt,
                ChangedBy = x.ChangedBy,
                IsDeleted = x.IsDeleted
            }).FirstOrDefaultAsync();

        if (response == default)
        {
            throw new Exception("Clubs couldn't find.");
        }

        return response;
    }

    public async Task<List<ClubResponseModel>> GetAll()
    {
        var response = await _applicationDbContext.Clubs
            .Select(x => new ClubResponseModel()
            {
                Id = x.Id,
                ClubName = x.ClubName,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                ChangedAt = x.ChangedAt,
                ChangedBy = x.ChangedBy,
                IsDeleted = x.IsDeleted
            }).ToListAsync();

        if (response == default)
        {
            throw new Exception("Club list couldn't find.");
        }

        return response;
    }

    public async Task<ClubResponseModel> Create(CreateClubRequestModel request)
    {
        var club = new Club()
        {
            ClubName = request.ClubName,
            CreatedAt = request.CreatedAt,
            CreatedBy = request.CreatedBy,
            ChangedAt = request.ChangedAt,
            ChangedBy = request.ChangedBy,
            IsDeleted = request.IsDeleted
        };

        if (club == default)
        {
            throw new Exception("Club couldn't created.");
        }

        _applicationDbContext.Add(club);
        await _applicationDbContext.SaveChangesAsync();

        var response = new ClubResponseModel()
        {
            Id = club.Id,
            ClubName = club.ClubName,
            CreatedAt = club.CreatedAt,
            CreatedBy = club.CreatedBy,
            ChangedAt = club.ChangedAt,
            ChangedBy = club.ChangedBy
        };

        return response;

    }

    public async Task<ClubResponseModel> Update(UpdateClubRequestModel request)
    {
        var club = await _applicationDbContext.Clubs
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();

        if (club == default)
        {
            throw new Exception("Club couldn't find.");
        }

        if (request.ClubName is not null) club.ClubName = request.ClubName;
        if (request.ChangedAt is not null) club.ChangedAt = request.ChangedAt;
        if (request.ChangedBy is not null) club.ChangedBy = request.ChangedBy;
        if (request.IsDeleted is not null) club.IsDeleted = (bool)request.IsDeleted;

        await _applicationDbContext.SaveChangesAsync();

        var response = new ClubResponseModel()
        {
            Id = club.Id,
            ClubName = club.ClubName,
            CreatedAt = club.CreatedAt,
            CreatedBy = club.CreatedBy,
            ChangedAt = club.ChangedAt,
            ChangedBy = club.ChangedBy,
            IsDeleted = club.IsDeleted
        };

        return response;
        
    }
}