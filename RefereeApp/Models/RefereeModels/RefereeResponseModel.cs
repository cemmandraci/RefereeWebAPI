using RefereeApp.Entities;
using RefereeApp.Models.RefereeModels.RefereeRegions;
using RefereeApp.Models.RefLevels;

namespace RefereeApp.Models.RefereeModels;

public class RefereeResponseModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int PhoneNumber { get; set; }
    public int RefLevelId { get; set; }
    public int RefRegionId { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public RefereeRegionResponseModel RefereeRegion { get; set; }
    public RefereeLevelsResponseModel RefereeLevel { get; set; }
}