using FluentValidation;

namespace RefereeApp.Models.RefereeModels.RefLevels;

public class UpdateRefereeLevelsRequestModel
{
    public int Id { get; set; }
    public int? StatusLevel { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; } = DateTime.Now;
    public string ChangedBy { get; set; }
    public bool? IsDeleted { get; set; }
}

public class UpdateRefereeLevelsRequestModelValidator : AbstractValidator<UpdateRefereeLevelsRequestModel>
{
    public UpdateRefereeLevelsRequestModelValidator()
    {
        RuleFor(x => x.StatusLevel).GreaterThan(0).LessThanOrEqualTo(10);
    }
}