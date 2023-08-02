#nullable enable
using FluentValidation;
using RefereeApp.Entities.Enums;

namespace RefereeApp.Models.FixtureModels;

public class CreateFixtureRequestModel
{
    public string? HomeTeam { get; set; }
    public string? AwayTeam { get; set; }
    public DateTime MatchTime { get; set; }
    public Difficulty DifficultyId { get; set; }
    public bool IsDerby { get; set; }
    public int RefId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; } = DateTime.Now;
    public string? ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

public class CreateFixtureRequestModelValidator : AbstractValidator<CreateFixtureRequestModel>
{
    public CreateFixtureRequestModelValidator()
    {
        RuleFor(x => x.AwayTeam).NotEmpty().MaximumLength(20);
        RuleFor(x => x.HomeTeam).NotEmpty().MaximumLength(20);
        RuleFor(x => x.RefId).NotEmpty();
        RuleFor(x => x.DifficultyId).IsInEnum();
    }
}