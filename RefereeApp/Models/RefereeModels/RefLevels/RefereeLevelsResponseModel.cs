#nullable enable
namespace RefereeApp.Models.RefLevels;

public class RefereeLevelsResponseModel
{
    public int Id { get; set; }
    public int? StatusLevel { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string? ChangedBy { get; set; }
    public bool? IsDeleted { get; set; }
}