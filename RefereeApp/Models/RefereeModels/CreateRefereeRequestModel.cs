using FluentValidation;
using RefereeApp.Models.RefereeModels.RefLevels;

namespace RefereeApp.Models.RefereeModels;

public class CreateRefereeRequestModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int PhoneNumber { get; set; }
    public bool IsActive { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public CreateRefereeRegionRequestModel RefereeRegion { get; set; }
    public CreateRefereeLevelsRequestModel RefereeLevels { get; set; }
}

public class CreateRefereeRequestModelValidator : AbstractValidator<CreateRefereeRequestModel>
{
    public CreateRefereeRequestModelValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.IsActive).NotEmpty();
        RuleFor(x => x.CreatedBy).NotEmpty();
        RuleFor(x => x.RefereeRegion).SetValidator(x => new CreateRefereeRegionRequestModelValidator());
        RuleFor(x => x.RefereeLevels).SetValidator(x => new CreateRefereeLevelsRequestModelValidator());
    }
}