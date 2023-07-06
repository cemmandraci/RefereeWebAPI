using FluentValidation;

namespace RefereeApp.Models.ClubModels;

public class UpdateClubRequestModel
{
    public int Id { get; set; }
    public string? ClubName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string ChangedBy { get; set; }
    public bool? IsDeleted { get; set; } = false;
}

public class UpdateClubRequestModelValidator : AbstractValidator<UpdateClubRequestModel>
{
    public UpdateClubRequestModelValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ChangedAt).NotEmpty();
        RuleFor(x => x.ChangedBy).NotEmpty();
    }
}