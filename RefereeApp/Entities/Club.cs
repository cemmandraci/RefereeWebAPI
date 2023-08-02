namespace RefereeApp.Entities;

public class Club : Base
{
    public int Id { get; set; }
    public string ClubName { get; set; }
    public Fixture Fixture { get; set; }
}