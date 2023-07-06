using FluentValidation;

namespace RefereeApp.Models.ClubModels;

public class UpdateClubRequestModel
{
    public int Id { get; set; }
    public string? ClubName { get; set; }
}

public class UpdateClubRequestModelValidator : AbstractValidator<UpdateClubRequestModel>
{
    public UpdateClubRequestModelValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}