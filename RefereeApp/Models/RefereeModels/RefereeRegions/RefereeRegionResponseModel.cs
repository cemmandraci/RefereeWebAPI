using RefereeApp.Entities.Enums;

namespace RefereeApp.Models.RefereeModels.RefereeRegions;

public class RefereeRegionResponseModel
{
    public int Id { get; set; }
    public Region RegionId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}