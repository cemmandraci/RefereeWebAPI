using FluentValidation;
using RefereeApp.Models.RefereeModels.RefLevels;
using RefereeApp.Models.RefLevels;

namespace RefereeApp.Models.RefereeModels;

public class UpdateRefereeRequestModel
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public bool? IsActive { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.Now;
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public UpdateRefereeLevelsRequestModel RefereeLevel { get; set; }
    public UpdateRefereeRegionRequestModel RefereeRegion { get; set; }
}

public class UpdateRefereeRequestModelValidator : AbstractValidator<UpdateRefereeRequestModel>
{
    public UpdateRefereeRequestModelValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.RefereeLevel).SetValidator(x => new UpdateRefereeLevelsRequestModelValidator());
        RuleFor(x => x.RefereeRegion).SetValidator(x => new UpdateRefereeRegionRequestModelValidator());
    }
}