namespace RefereeApp.Models.RefereeModels.RefereeMatches;

public class RefereeMatchResponseModel
{
    public int Id { get; set; }
    public int RefereeId { get; set; }
    public DateTime LastAttendMatch { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; }
}