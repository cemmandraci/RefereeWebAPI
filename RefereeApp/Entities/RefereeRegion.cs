using RefereeApp.Entities.Enums;

namespace RefereeApp.Entities;

public class RefereeRegion : Base
{
    public int Id { get; set; }
    public Region RegionId { get; set; }
    public Referee Referee { get; set; }
}