using Azure.Core;
using Microsoft.EntityFrameworkCore;
using RefereeApp.Abstractions;
using RefereeApp.Data;
using RefereeApp.Entities;
using RefereeApp.Entities.Enums;
using RefereeApp.Exceptions;
using RefereeApp.Models.FixtureModels;

namespace RefereeApp.Concretes;

public class FixtureService : IFixtureService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<FixtureService> _logger;
    private readonly IAuthService _authService;

    public FixtureService(ApplicationDbContext applicationDbContext, ILogger<FixtureService> logger, IAuthService authService)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
        _authService = authService;
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
            throw new NotFoundException("An error occured !");
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
            throw new NotFoundException("An error occured !");
        }
        
        _logger.LogInformation("Fixture Get() | Club list fetched successfully.");

        return response;
    }

    public async Task<FixtureResponseModel> Create(CreateFixtureRequestModel request)
    {
        var username = _authService.GetUsernameFromToken();
        _logger.LogInformation("Fixture Create() | Function is starting.");
        
        var refereeControl = await _applicationDbContext.Referees
            .AsNoTracking()
            .Include(x=>x.RefereeLevel)
            .Include(x=>x.RefereeMatch)
            .Where(x => x.Id == request.RefId && !x.IsDeleted)
            .FirstOrDefaultAsync();

        var clubNames = await _applicationDbContext.Clubs
            .AsNoTracking()
            .Select(x=>x.ClubName.ToLower())
            .ToListAsync();

        _logger.LogInformation("Club control is starting.");
        if (request.HomeTeam != null && !clubNames.Contains(request.HomeTeam.ToLower()) || request.AwayTeam != null && !clubNames.Contains(request.AwayTeam.ToLower()))
        {
            _logger.LogError("Fixture Create() | Home team or away team doesn't exist in club list.");
            throw new BadRequestException("An error occured !");
        }
        
        _logger.LogInformation("Fixture Create() | To checking referee is starting.");
        if (refereeControl == default)
        {
            _logger.LogError("Fixture Create() | There isn't exist {refereeId} referee, fetch is failed.",request.RefId);
            throw new NotFoundException("An error occured !");
        }
        
        _logger.LogInformation("Fixture Create() | To checking teams uniqueness is starting.");
        var teamControl = IsUnique(request.HomeTeam, request.AwayTeam);

        if (!teamControl)
        {
            _logger.LogError("Fixture Create() | Teams can not be the same.");
            throw new BadRequestException("An error occured !");
        }

        if (!refereeControl.IsActive)
        {
            _logger.LogError("Fixture Create() | The referee cannot be attend in this match, his activiness is false.");
            throw new BadRequestException("An error occured !");
        }

        var refereeMatchTime = refereeControl.RefereeMatch.Select(x => x.LastAttendMatch).FirstOrDefault();

        _logger.LogInformation("Fixture Create() | To checking referee activiness is starting.");
        var check = CanAttend(refereeMatchTime, request.MatchTime);

        if (!check)
        {
            _logger.LogError("Fixture Create() | The referee cannot be attend in this match.");
            throw new BadRequestException("An error occured !");
        }

        _logger.LogInformation("Fixture Create() | To checking difficultiness of match is starting.");
        var difficultyControl = refereeControl.RefereeLevel.StatusLevel != null && StatusOfMatch((int)refereeControl.RefereeLevel.StatusLevel, request.DifficultyId,
            request.IsDerby);

        if (!difficultyControl)
        {
            _logger.LogError("Fixture Create() | Referee can't assign the match.Match difficulty {diff} is not match with the ref status level", request.DifficultyId);
            throw new BadRequestException("An error occured !");
        }


        _logger.LogInformation("Fixture Create() | Fixture entity is starting to create.");
        var fixture = new Fixture()
        {
            HomeTeam = request.HomeTeam!.ToUpper(),
            AwayTeam = request.AwayTeam!.ToUpper(),
            IsDerby = request.IsDerby,
            DifficultyId = request.DifficultyId,
            RefId = request.RefId,
            MatchTime = request.MatchTime,
            CreatedAt = DateTime.Now,
            CreatedBy = username,
            ChangedAt = DateTime.Now,
            ChangedBy = username,
            IsDeleted = request.IsDeleted
        };

        if (fixture == default)
        {
            _logger.LogError("Fixture Create() | Fixture entity couldn't created.");
            throw new BadRequestException("An error occured !");
        }

        var newRefereeMatch = new RefereeMatch()
        {
            RefereeId = request.RefId,
            LastAttendMatch = request.MatchTime,
            CreatedAt = DateTime.Now,
            CreatedBy = username,
            ChangedAt = DateTime.Now,
            ChangedBy = username,
            IsDeleted = false
        };

        refereeControl.LastAttendMatch = request.MatchTime;

        _applicationDbContext.Referees.Update(refereeControl);
        _applicationDbContext.RefereeMatches.Add(newRefereeMatch);
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
        var username = _authService.GetUsernameFromToken();
        _logger.LogInformation("Fixture Update() | Function is starting.");
        _logger.LogInformation("Fixture Update() | To finding entity from db is starting.");

        _logger.LogInformation("Fixture Update() | Fixture of referee entity is pulling from db");
        var referee = await _applicationDbContext.Referees
            .Include(x=>x.RefereeMatch)
            .Include(x=>x.RefereeLevel)
            .Where(x => x.Id == request.RefId)
            .FirstOrDefaultAsync();

        _logger.LogInformation("Fixture Update() | Fixture entity is pulling from db");
        var entity = await _applicationDbContext.Fixtures
            .Include(x=>x.Referee)
            .ThenInclude(x=>x.RefereeMatch)
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();
        
        var clubNames = await _applicationDbContext.Clubs
            .AsNoTracking()
            .Select(x=>x.ClubName.ToLower())
            .ToListAsync();
        
        if (entity == default)
        {
            _logger.LogError("Fixture Update() | Fetching {id} entity is failed.", request.Id);
            throw new NotFoundException("An error occured !");
        }
        
        _logger.LogInformation("Club control is starting.");
        if (request.HomeTeam != null && !clubNames.Contains(request.HomeTeam.ToLower()) || request.AwayTeam != null && !clubNames.Contains(request.AwayTeam.ToLower()))
        {
            _logger.LogError("Fixture Update() | Home team or away team doesn't exist in club list.");
            throw new BadRequestException("An error occured !");
        }
        
        _logger.LogInformation("Fixture Update() | To checking teams uniqueness is starting.");
        
        if (request.HomeTeam is not null) entity.HomeTeam = request.HomeTeam.ToUpper();
        if (request.AwayTeam is not null) entity.AwayTeam = request.AwayTeam.ToUpper();
        
        var teamControl = IsUnique(entity.HomeTeam, entity.AwayTeam);
        if (!teamControl)
        {
            _logger.LogError("Fixture Update() | Teams can not be same.");
            throw new BadRequestException("An error occured !");        
        }

        _logger.LogInformation("Fixture Update() | Fixture match time and referee id change control is starting.");
        if ((request.MatchTime is not null && entity.MatchTime != request.MatchTime) ||
            (request.RefId != entity.RefId))
        {
            if (referee.IsActive && referee.Id != entity.RefId)
            {
                var entityRefereeMatches = entity.Referee.RefereeMatch.OrderByDescending(y => y.LastAttendMatch).ToList();
                var deleteMatch = entity.Referee.RefereeMatch.MaxBy(x => x.LastAttendMatch);

                var refereeMatchTime = referee.RefereeMatch
                    .OrderByDescending(x => x.LastAttendMatch)
                    .Where(x => x.LastAttendMatch < entity.MatchTime)
                    .Select(x => x.LastAttendMatch)
                    .FirstOrDefault();
                
                _logger.LogInformation("Fixture Update() | To checking referee activiness is starting.");
                if (request.MatchTime is not null) entity.MatchTime = (DateTime)request.MatchTime;
                var refereeControl = CanAttend(refereeMatchTime, entity.MatchTime);

                if (refereeControl)
                {
                    _logger.LogInformation("Fixture Update() | New referee match is starting to add to its own referee fixture.");
                    var newRefereeMatch = new RefereeMatch()
                    {
                        RefereeId = (int)request.RefId!,
                        LastAttendMatch = entity.MatchTime,
                        CreatedAt = DateTime.Now,
                        CreatedBy = username,
                        ChangedAt = DateTime.Now,
                        ChangedBy = username,
                        IsDeleted = false
                    };

                    _applicationDbContext.RefereeMatches.Add(newRefereeMatch);

                    if (entityRefereeMatches.Count > 1)
                    {
                        var secondRefereeMatch = entityRefereeMatches.ElementAtOrDefault(1);
                        if (secondRefereeMatch != null)
                            entity.Referee.LastAttendMatch = secondRefereeMatch.LastAttendMatch;
                        deleteMatch.IsDeleted = true;
                    }
                    else
                    {
                        entity.Referee.LastAttendMatch = entity.MatchTime;
                        deleteMatch.IsDeleted = true;
                    }

                    entity.RefId = (int)request.RefId;
                    referee.LastAttendMatch = referee.LastAttendMatch > entity.MatchTime
                        ? referee.LastAttendMatch
                        : entity.MatchTime;
                }
                else
                {
                    _logger.LogError("Fixture Update() | Referee can not attend this match !");
                    throw new BadRequestException("An error occured !");
                }
            }
            else
            {
                var lastRefereeMatch = referee.RefereeMatch.MaxBy(x => x.LastAttendMatch);
                lastRefereeMatch.LastAttendMatch = (DateTime)request.MatchTime!;
                entity.Referee.LastAttendMatch = request.MatchTime;
                entity.MatchTime = (DateTime)request.MatchTime;
            }
        }
        
        entity.IsDerby = request.IsDerby;
        entity.DifficultyId = request.DifficultyId;
        
        _logger.LogInformation("Fixture Update() | Difficulty control is starting");
        var difficultyControl = referee.RefereeLevel.StatusLevel != null &&
                                StatusOfMatch((int)referee.RefereeLevel.StatusLevel, entity.DifficultyId,
                                    entity.IsDerby);

        if (!difficultyControl)
        {
            _logger.LogError("Fixture Update() | Referee can't assign the match.Match difficulty {diff} is not match with the ref status level", request.DifficultyId);
            throw new BadRequestException("An error occured !");
        }

        if (request.ChangedAt is not null) entity.ChangedAt = DateTime.Now;
        if (request.ChangedBy is not null) entity.ChangedBy = username;
        
        
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
            throw new BadRequestException("An error occured !");
        }

        return true;
    }
    
    private static bool CanAttend(DateTime lastAttend, DateTime matchTime)
    {
        var onlyLastAttend = new DateTime(lastAttend.Year, lastAttend.Month, lastAttend.Day);
        var onlyMatchTime = new DateTime(matchTime.Year, matchTime.Month, matchTime.Day);
        
        return (onlyMatchTime - onlyLastAttend).TotalDays >= 2;
    }

    private static bool StatusOfMatch(int statusLevel , Difficulty status, bool isDerby)
    {
        if (isDerby)
        {
            if (statusLevel is 10)
            {
                return true;
            }
        }
        else
        {
            switch (status)
            {
                case  Difficulty.Easy:
                    return true;
                case Difficulty.Medium:
                    if (statusLevel is >= 1 and <= 4)
                    {
                        return true;
                    }

                    break;
                case Difficulty.Hard:
                    if (statusLevel is >= 5 and <= 8 or > 8)
                    {
                        return true;
                    }

                    break;
                case Difficulty.VeryDifficult:
                    if (statusLevel is 9 or 10 )
                    {
                        return true;
                    }

                    break;
                default:
                    return false;
            }

            return false;
        }

        return false;

    }
    
}