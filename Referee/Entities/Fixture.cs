using Referee.Entities.Enums;

namespace Referee.Entities;

public class Fixture : Base
{
    public int Id { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public DateTime MatchTime { get; set; }
    public Difficulty DifficultyId { get; set; }
    public bool IsDerby { get; set; }
    public Referee RefereeId { get; set; }
}