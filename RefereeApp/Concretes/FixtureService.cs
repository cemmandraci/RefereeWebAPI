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

    public FixtureService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<FixtureResponseModel> Get(int id)
    {
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
            throw new Exception("Fixture couldn't find");
        }

        return response;
    }

    public async Task<List<FixtureResponseModel>> GetAll()
    {
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
            throw new Exception("Fixture list couldn't find.");
        }

        return response;
    }

    public async Task<FixtureResponseModel> Create(CreateFixtureRequestModel request)
    {
        
        // TODO : Club ile ilgili ne yapacaksın ?
        // TODO : Club servisi için api yazılacak ayrı arayüzü olacak takım eklenecek çıkarılacak.
        // TODO : Home ve away teamler club tablosunun içinde olmak zorunda , ya fleunt api ile yapılacak yada fonksiyon yazılacak
        // TODO : isDeleted alanları için kontrollere eklemeyi unutma ! // DONE //
        // TODO : isUnique() fonksiyonu için takımların büyük/küçük harf duyarlılıgına önlem almayı unutma ! // DONE //
        // TODO : Derbi maç ve difficulty kısıtlamalarını eklemeyi unutma ! // DONE //
        // TODO : Program.cs dosyasında fluent api için migration değişti kontrol edilecek ! // DONE //

        var refereeControl = await _applicationDbContext.Referees
            .AsNoTracking()
            .Include(x=>x.RefereeLevel)
            .Where(x => x.Id == request.RefId && !x.IsDeleted)
            .FirstOrDefaultAsync();


        if (refereeControl == default)
        {
            throw new Exception("Referee couldn't found.");
        }
        
        var teamControl = IsUnique(request.HomeTeam, request.AwayTeam);

        if (!teamControl)
        {
            throw new Exception("Teams can not be the same.");
        }

        if (refereeControl.IsActive)
        {
            var check = CanAttend(request.CreatedAt, request.MatchTime);

            if (!check)
            {
                throw new Exception("You can not attend referee in this match");
            }
        }

        var difficultyControl = refereeControl.RefereeLevel.StatusLevel != null && StatusOfMatch((int)refereeControl.RefereeLevel.StatusLevel, request.DifficultyId,
            request.IsDerby);

        if (!difficultyControl)
        {
            throw new Exception("You can't assign the refeere. Match diffuculty is not match with the ref status level.");
        }
       

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
            throw new Exception("Fixture couldn't created.");
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

        return response;

    }

    public async Task<FixtureResponseModel> Update(UpdateFixtureRequestModel request)
    {
        var entity = await _applicationDbContext.Fixtures.Include(x=>x.Referee).Where(x => x.Id == request.Id).FirstOrDefaultAsync();

        if (entity == default)
        {
            throw new Exception("Entity couldn't find.");
        }

        var teamControl = IsUnique(request.HomeTeam, request.AwayTeam);

        if (teamControl)
        {
            if (request.HomeTeam is not null) entity.HomeTeam = request.HomeTeam;
            if (request.AwayTeam is not null) entity.AwayTeam = request.AwayTeam;
        }

        if (entity.Referee.IsActive)
        {
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