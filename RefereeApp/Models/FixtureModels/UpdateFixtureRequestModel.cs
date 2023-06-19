using FluentValidation;
using RefereeApp.Entities;
using RefereeApp.Entities.Enums;

namespace RefereeApp.Models.FixtureModels;

public class UpdateFixtureRequestModel
{
    public int Id { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public DateTime MatchTime { get; set; }
    public Difficulty DifficultyId { get; set; }
    public bool IsDerby { get; set; }
    public int RefId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public Referee Referee { get; set; }
}

public class UpdateFixtureRequestModelValidator : AbstractValidator<UpdateFixtureRequestModel>
{
    public UpdateFixtureRequestModelValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ChangedAt).NotEmpty();
        RuleFor(x => x.ChangedBy).NotEmpty();
        RuleFor(x => x.DifficultyId).IsInEnum();
    }
}