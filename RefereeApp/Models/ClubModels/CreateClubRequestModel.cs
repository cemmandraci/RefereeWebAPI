using FluentValidation;

namespace RefereeApp.Models.ClubModels;

public class CreateClubRequestModel
{
    public string ClubName { get; set; }
}

public class CreateClubRequestModelValidator : AbstractValidator<CreateClubRequestModel>
{
    public CreateClubRequestModelValidator()
    {
        RuleFor(x => x.ClubName).NotEmpty();
    }
}