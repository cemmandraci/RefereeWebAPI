using Microsoft.EntityFrameworkCore;
using RefereeApp.Abstractions;
using RefereeApp.Data;
using RefereeApp.Entities;
using RefereeApp.Exceptions;
using RefereeApp.Models.ClubModels;

namespace RefereeApp.Concretes;

public class ClubService : IClubService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<ClubService> _logger;
    private readonly IAuthService _authService;

    public ClubService(ApplicationDbContext applicationDbContext, ILogger<ClubService> logger, IAuthService authService)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
        _authService = authService;
    }

    //TODO : Club - Genel test çevrilecek.
    public async Task<ClubResponseModel> GetById(int id)
    {
        _logger.LogInformation("Club GetById() | Function is starting.");
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
            _logger.LogError("Club GetById() | There is no any {id} club entity, fetch is failed.", id);
            throw new NotFoundException("An error occured !");
        }
        
        _logger.LogInformation("Club GetById() | Fetch is successful.");

        return response;
    }

    public async Task<List<ClubResponseModel>> Get()
    {
        _logger.LogInformation("Club Get() | Function is starting.");
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
            _logger.LogError("Club Get() | There aren't exist club list, fetch is failed.");
            throw new NotFoundException("An error occured !");
        }

        _logger.LogInformation("Fixture Get() | Club list fetched successfully.");
        return response;
    }

    public async Task<ClubResponseModel> Create(CreateClubRequestModel request)
    {
        var userId = _authService.GetUserIdFromToken();
        var isExist = await _applicationDbContext.Clubs
            .AsNoTracking()
            .Where(x => x.ClubName == request.ClubName)
            .Select(x=>x.ClubName)
            .FirstOrDefaultAsync();

        if (isExist == request.ClubName)
        {
            _logger.LogError("Club name already exist !");
            throw new BadRequestException("An error occured !");
        }
        
        _logger.LogInformation("Club Create() | Function is starting");
        var club = new Club()
        {
            ClubName = request.ClubName,
            CreatedAt = DateTime.Now,
            CreatedBy = userId,
            ChangedAt = DateTime.Now,
            ChangedBy = userId,
            IsDeleted = request.IsDeleted
        };

        if (club == default)
        {
            _logger.LogError("Club Create() | Club entity couldn't created.");
            throw new NotFoundException("An error occured !");
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
        
        _logger.LogInformation("Club Create() | Club entity is created successfully.");
        return response;

    }

    public async Task<ClubResponseModel> Update(UpdateClubRequestModel request)
    {
        var userId = _authService.GetUserIdFromToken();
        _logger.LogInformation("Club Update() | Function is starting.");
        _logger.LogInformation("Club Update() | To finding entity from db is starting.");
        var club = await _applicationDbContext.Clubs
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();

        if (club == default)
        {
            _logger.LogError("Club Update() | Fetching {id} entity is failed.", request.Id);
            throw new NotFoundException("An error occured !");
        }

        if (club.ClubName == request.ClubName )
        {
            _logger.LogError("Club name already exist !");
            throw new BadRequestException("An error occured !");
        }

        if (request.ClubName is not null) club.ClubName = request.ClubName;
        if (request.ChangedAt is not null) club.ChangedAt = DateTime.Now;
        if (request.ChangedBy is not null) club.ChangedBy = userId;
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
        
        _logger.LogInformation("Club Update() | Club entity is updated successfully.");

        return response;
        
    }
}