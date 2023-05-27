namespace RefereeApp.Entities;

public class RefereeLevel : Base
{
    public int Id { get; set; }
    public int StatusLevel { get; set; }
    public Referee Referee { get; set; }
}