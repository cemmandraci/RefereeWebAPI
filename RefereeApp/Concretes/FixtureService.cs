using Microsoft.EntityFrameworkCore;
using RefereeApp.Abstractions;
using RefereeApp.Data;
using RefereeApp.Entities;
using RefereeApp.Entities.Enums;
using RefereeApp.Models.FixtureModels;

namespace RefereeApp.Concretes;

public class FixtureService : IFixtureService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<FixtureService> _logger;

    public FixtureService(ApplicationDbContext applicationDbContext, ILogger<FixtureService> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    public async Task<FixtureResponseModel> GetById(int id)
    {
        _logger.LogInformation("Fixture Get() | Function is starting.");
        var response = await _applicationDbContext.Fixtures
            .Where(x => x.Id == id)
            .Select(x => new FixtureResponseModel()
            {
                Id = x.Id,
                HomeTeam = x.HomeTeam,
                AwayTeam = x.AwayTeam,
                IsDerby = x.IsDerby,
                DifficultyId = x.DifficultyId,
                RefId = x.RefId,
                MatchTime = x.MatchTime,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                ChangedAt = x.ChangedAt,
                ChangedBy = x.ChangedBy
            }).FirstOrDefaultAsync();

        if (response == default)
        {
            _logger.LogError("Fixture Get() | There is no any {id} fixture entity, fetch is failed.", id);
            throw new Exception("Something wrong.");
        }
        
        _logger.LogInformation("Fixture Get() | Fetch is successful.");

        return response;
    }

    public async Task<List<FixtureResponseModel>> Get()
    {
        _logger.LogInformation("Fixture Get() | Function is starting.");
        var response = await _applicationDbContext.Fixtures
            .AsNoTracking()
            .Select(x => new FixtureResponseModel()
            {
                Id = x.Id,
                HomeTeam = x.HomeTeam,
                AwayTeam = x.AwayTeam,
                IsDerby = x.IsDerby,
                DifficultyId = x.DifficultyId,
                RefId = x.RefId,
                MatchTime = x.MatchTime,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                ChangedAt = x.ChangedAt,
                ChangedBy = x.ChangedBy
            }).ToListAsync();

        if (response == default)
        {
            _logger.LogError("Fixture Get() | There isn't exist fixture list, fetch is failed.");
            throw new Exception("Something wrong.");
        }
        
        _logger.LogInformation("Fixture Get() | Club list fetched successfully.");

        return response;
    }

    public async Task<FixtureResponseModel> Create(CreateFixtureRequestModel request)
    {
        _logger.LogInformation("Fixture Create() | Function is starting.");
        
        // TODO : Club ile ilgili ne yapacaksın ? //DONE//
        // TODO : Club servisi için api yazılacak ayrı arayüzü olacak takım eklenecek çıkarılacak. //DONE//
        // TODO : Home ve away teamler club tablosunun içinde olmak zorunda , ya fleunt api ile yapılacak yada fonksiyon yazılacak
        // TODO : isDeleted alanları için kontrollere eklemeyi unutma ! // DONE //
        // TODO : isUnique() fonksiyonu için takımların büyük/küçük harf duyarlılıgına önlem almayı unutma ! // DONE //
        // TODO : Derbi maç ve difficulty kısıtlamalarını eklemeyi unutma ! // DONE //
        // TODO : Program.cs dosyasında fluent api için migration değişti kontrol edilecek ! // DONE //

        _logger.LogInformation("Fixture Create() | To checking referee is starting.");
        var refereeControl = await _applicationDbContext.Referees
            .AsNoTracking()
            .Include(x=>x.RefereeLevel)
            .Where(x => x.Id == request.RefId && !x.IsDeleted)
            .FirstOrDefaultAsync();


        if (refereeControl == default)
        {
            _logger.LogError("Fixture Create() | There isn't exist {refereeId} referee, fetch is failed.",request.RefId);
            throw new Exception("Something wrong.");
        }
        
        _logger.LogInformation("Fixture Create() | To checking teams uniqueness is starting.");
        var teamControl = IsUnique(request.HomeTeam, request.AwayTeam);

        if (!teamControl)
        {
            _logger.LogError("Fixture Create() | Teams can not be the same.");
            throw new Exception("Something wrong.");
        }
        
        if (refereeControl.IsActive)
        {
            _logger.LogInformation("Fixture Create() | To checking referee activiness is starting.");
            var check = CanAttend(request.CreatedAt, request.MatchTime);

            if (!check)
            {
                _logger.LogError("Fixture Create() | The referee cannot be attend in this match.");
                throw new Exception("Something wrong.");
            }
        }

        _logger.LogInformation("Fixture Create() | To checking difficultiness of match is starting.");
        var difficultyControl = refereeControl.RefereeLevel.StatusLevel != null && StatusOfMatch((int)refereeControl.RefereeLevel.StatusLevel, request.DifficultyId,
            request.IsDerby);

        if (!difficultyControl)
        {
            _logger.LogError("Fixture Create() | Referee can't assign the match.Match difficulty {diff} is not match with the ref status level", request.DifficultyId);
            throw new Exception("Something wrong.");
        }
       

        _logger.LogInformation("Fixture Create() | Fixture entity is starting to create.");
        var fixture = new Fixture()
        {
            HomeTeam = request.HomeTeam,
            AwayTeam = request.AwayTeam,
            IsDerby = request.IsDerby,
            DifficultyId = request.DifficultyId,
            RefId = request.RefId,
            MatchTime = request.MatchTime,
            CreatedAt = request.CreatedAt,
            CreatedBy = request.CreatedBy,
            ChangedAt = request.ChangedAt,
            ChangedBy = request.ChangedBy,
            IsDeleted = request.IsDeleted
        };

        if (fixture == default)
        {
            _logger.LogError("Fixture Create() | Fixture entity couldn't created.");
            throw new Exception("Something wrong.");
        }

        _applicationDbContext.Add(fixture);
        await _applicationDbContext.SaveChangesAsync();

        var response = new FixtureResponseModel()
        {
            Id = fixture.Id,
            HomeTeam = fixture.HomeTeam,
            AwayTeam = fixture.AwayTeam,
            IsDerby = fixture.IsDerby,
            DifficultyId = fixture.DifficultyId,
            RefId = fixture.RefId,
            MatchTime = fixture.MatchTime,
            CreatedAt = fixture.CreatedAt,
            CreatedBy = fixture.CreatedBy,
            ChangedAt = fixture.ChangedAt,
            ChangedBy = fixture.ChangedBy,
            IsDeleted = fixture.IsDeleted,
        };
        
        _logger.LogInformation("Fixture Create() | Fixture entity is created successfully.");

        return response;

    }

    public async Task<FixtureResponseModel> Update(UpdateFixtureRequestModel request)
    {
        _logger.LogInformation("Fixture Update() | Function is starting.");
        _logger.LogInformation("Fixture Update() | To finding entity from db is starting.");
        var entity = await _applicationDbContext.Fixtures.Include(x=>x.Referee).Where(x => x.Id == request.Id).FirstOrDefaultAsync();

        if (entity == default)
        {
            _logger.LogError("Fixture Update() | Fetching {id} entity is failed.", request.Id);
            throw new Exception("Something wrong.");
        }
        
        _logger.LogInformation("Fixture Update() | To checking teams uniqueness is starting.");
        var teamControl = IsUnique(request.HomeTeam, request.AwayTeam);

        if (teamControl)
        {
            if (request.HomeTeam is not null) entity.HomeTeam = request.HomeTeam;
            if (request.AwayTeam is not null) entity.AwayTeam = request.AwayTeam;
        }
        else
        {
            _logger.LogError("Fixture Update() | Teams can not be same.");
        }

        if (entity.Referee.IsActive)
        {
            _logger.LogInformation("Fixture Update() | To checking referee activiness is starting.");
            var refereeControl = CanAttend(entity.CreatedAt, request.MatchTime);

            if (refereeControl)
            {
                entity.RefId = request.RefId;
            }
        }

        if (request.ChangedAt is not null) entity.ChangedAt = request.ChangedAt;
        if (request.ChangedBy is not null) entity.ChangedBy = request.ChangedBy;
        entity.IsDerby = request.IsDerby;
        entity.MatchTime = request.MatchTime;

        await _applicationDbContext.SaveChangesAsync();

        var response = new FixtureResponseModel()
        {
            Id = entity.Id,
            HomeTeam = entity.HomeTeam,
            AwayTeam = entity.AwayTeam,
            IsDerby = entity.IsDerby,
            DifficultyId = entity.DifficultyId,
            RefId = entity.RefId,
            MatchTime = entity.MatchTime,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            ChangedAt = entity.ChangedAt,
            ChangedBy = entity.ChangedBy,
            IsDeleted = entity.IsDeleted
        };
        
        _logger.LogInformation("Fixture Update() | Fixture entity is updated successfully.");

        return response;
    }

    private static bool IsUnique(string firstClub, string secondClub)
    {
        if (string.Equals(firstClub, secondClub, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new Exception("Teams can not be the same.");
        }

        return true;
    }
    
    private static bool CanAttend(DateTime createdAt, DateTime matchTime)
    {
        var onlyDate = createdAt.Day;
        var matchTimeDate = matchTime.Day;
        var difference = matchTimeDate - onlyDate;

        return difference >= 2;
    }

    private static bool StatusOfMatch(int statusLevel , Difficulty status, bool isDerby)
    {
        switch (statusLevel)
        {
            case <= 10 when status == Difficulty.Easy && !isDerby:
            case < 5 when status == Difficulty.Medium && !isDerby:
            case < 9 when status == Difficulty.Hard && !isDerby:
            case <= 10 when status == Difficulty.VeryDifficult && !isDerby:
            case 10 when isDerby:
                return true;
            default:
                return false;
        }
    }
    
}