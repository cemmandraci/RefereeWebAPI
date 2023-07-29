using FluentValidation;
using RefereeApp.Models.RefereeModels.RefereeRegions;
using RefereeApp.Models.RefereeModels.RefLevels;

namespace RefereeApp.Models.RefereeModels;

public class CreateRefereeRequestModel
{
    public bool IsActive { get; set; } = false;
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; } = DateTime.Now;
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public CreateRefereeRegionRequestModel RefereeRegion { get; set; }
    public CreateRefereeLevelsRequestModel RefereeLevels { get; set; }
}

public class CreateRefereeRequestModelValidator : AbstractValidator<CreateRefereeRequestModel>
{
    public CreateRefereeRequestModelValidator()
    {
        RuleFor(x => x.RefereeRegion).SetValidator(x => new CreateRefereeRegionRequestModelValidator());
        RuleFor(x => x.RefereeLevels).SetValidator(x => new CreateRefereeLevelsRequestModelValidator());
    }
}