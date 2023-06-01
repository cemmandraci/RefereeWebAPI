using FluentValidation;

namespace RefereeApp.Models.RefLevels;

public class UpdateRefereeLevelsRequestModel
{
    public int Id { get; set; }
    public int StatusLevel { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public bool IsDeleted { get; set; }
}

public class UpdateRefereeLevelsRequestModelValidator : AbstractValidator<UpdateRefereeLevelsRequestModel>
{
    public UpdateRefereeLevelsRequestModelValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ChangedAt).NotEmpty();
        RuleFor(x => x.ChangedBy).NotEmpty();
        RuleFor(x => x.StatusLevel).Must(Distance);
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