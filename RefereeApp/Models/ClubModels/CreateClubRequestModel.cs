using FluentValidation;

namespace RefereeApp.Models.ClubModels;

public class CreateClubRequestModel
{
    public string ClubName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

public class CreateClubRequestModelValidator : AbstractValidator<CreateClubRequestModel>
{
    public CreateClubRequestModelValidator()
    {
        RuleFor(x => x.ClubName).NotEmpty();
    }
}