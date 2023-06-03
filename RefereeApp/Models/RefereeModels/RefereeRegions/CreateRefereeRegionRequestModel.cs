using FluentValidation;
using RefereeApp.Entities.Enums;

namespace RefereeApp.Models.RefereeModels;

public class CreateRefereeRegionRequestModel
{
    public Region RegionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

public class CreateRefereeRegionRequestModelValidator : AbstractValidator<CreateRefereeRegionRequestModel>
{
    public CreateRefereeRegionRequestModelValidator()
    {
        RuleFor(x => x.RegionId).IsInEnum();
        RuleFor(x => x.CreatedBy).NotNull();
        RuleFor(x => x.CreatedAt).NotNull();
    }
}