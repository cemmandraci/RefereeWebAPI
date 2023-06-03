using FluentValidation;
using RefereeApp.Models.RefLevels;

namespace RefereeApp.Models.RefereeModels;

public class UpdateRefereeRequestModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public int RefLevelId { get; set; }
    public int RefRegionId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
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
        RuleFor(x => x.ChangedBy).NotEmpty();
        RuleFor(x => x.RefereeLevel).SetValidator(x => new UpdateRefereeLevelsRequestModelValidator());
        RuleFor(x => x.RefereeRegion).SetValidator(x => new UpdateRefereeRegionRequestModelValidator());
    }
}