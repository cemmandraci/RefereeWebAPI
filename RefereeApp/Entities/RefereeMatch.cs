namespace RefereeApp.Entities;

public class RefereeMatch : Base
{
    public int Id { get; set; }
    public int RefereeId { get; set; }
    public DateTime LastAttendMatch { get; set; }
    public Referee Referee { get; set; }
}