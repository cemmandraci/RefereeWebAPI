using RefereeApp.Models.RefereeModels.RefereeMatches;
using RefereeApp.Models.RefereeModels.RefereeRegions;
using RefereeApp.Models.RefLevels;

namespace RefereeApp.Models.RefereeModels;

public class RefereeResponseModel
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastAttendMatch { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; }
    public RefereeRegionResponseModel RefereeRegion { get; set; }
    public RefereeLevelsResponseModel RefereeLevel { get; set; }
    public List<RefereeMatchResponseModel> RefereeMatch { get; set; }
}