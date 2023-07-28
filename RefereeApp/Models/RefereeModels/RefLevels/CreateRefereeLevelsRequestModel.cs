#nullable enable
using FluentValidation;

namespace RefereeApp.Models.RefereeModels.RefLevels;

public class CreateRefereeLevelsRequestModel
{
    public int StatusLevel { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

public class CreateRefereeLevelsRequestModelValidator : AbstractValidator<CreateRefereeLevelsRequestModel>
{
    public CreateRefereeLevelsRequestModelValidator()
    {
        RuleFor(x => x.StatusLevel).NotEmpty().Must(Distance).WithMessage("StatusLevel must be between 1-10");
    }

    public bool Distance(int dist)
    {
        if (dist > 0 && dist <= 10)
        {
            return true;
        }
        return false;
    }
    
    
}