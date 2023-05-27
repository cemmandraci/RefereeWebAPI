using Referee.Entities.Enums;

namespace Referee.Entities;

public class RefereeRegion : Base
{
    public int Id { get; set; }
    public Region RegionId { get; set; }
}