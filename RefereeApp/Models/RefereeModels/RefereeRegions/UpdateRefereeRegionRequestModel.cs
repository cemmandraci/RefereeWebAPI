using FluentValidation;
using RefereeApp.Entities.Enums;

namespace RefereeApp.Models.RefereeModels;

public class UpdateRefereeRegionRequestModel
{
    public Region? RegionId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.Now;
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

public class UpdateRefereeRegionRequestModelValidator : AbstractValidator<UpdateRefereeRegionRequestModel>
{
    public UpdateRefereeRegionRequestModelValidator()
    {
        RuleFor(x => x.ChangedBy).NotEmpty();
        RuleFor(x => x.RegionId).IsInEnum();
    }
}