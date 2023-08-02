using FluentValidation;

namespace RefereeApp.Models.RefereeModels.RefereeMatches;

public class CreateRefereeMatchRequestModel
{
    //public int Id { get; set; }
    //public int RefereeId { get; set; }
    public DateTime LastAttendMatch { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; } = DateTime.Now;
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}