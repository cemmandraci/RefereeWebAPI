using FluentValidation;

namespace RefereeApp.Models.AuthModels;

public class LoginModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class LoginModelValidator : AbstractValidator<LoginModel>
{
    public LoginModelValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}