using FluentValidation;
using RefereeApp.Entities;

namespace RefereeApp.Models.RefereeModels;

public class CreateRefereeRequestModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public int RefLevelId { get; set; }
    public int RefRegionId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

public class CreateRefereeRequestModelValidator : AbstractValidator<CreateRefereeRequestModel>
{
    public CreateRefereeRequestModelValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.IsActive).NotEmpty();
        RuleFor(x => x.RefLevelId).NotEmpty();
        RuleFor(x => x.RefRegionId).NotEmpty();
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}