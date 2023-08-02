using FluentValidation;
using RefereeApp.Entities;
using RefereeApp.Entities.Enums;
using RefereeApp.Models.RefereeModels;

namespace RefereeApp.Models.FixtureModels;

public class UpdateFixtureRequestModel
{
    public int Id { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public DateTime? MatchTime { get; set; }
    public Difficulty DifficultyId { get; set; }
    public bool IsDerby { get; set; }
    public int RefId { get; set; }
    public DateTime? ChangedAt { get; set; } = DateTime.Now;
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

public class UpdateFixtureRequestModelValidator : AbstractValidator<UpdateFixtureRequestModel>
{
    public UpdateFixtureRequestModelValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.RefId).NotEmpty();
        RuleFor(x => x.DifficultyId).IsInEnum();
    }
}