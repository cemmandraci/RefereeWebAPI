namespace RefereeApp.Entities;

public class FixtureClub
{
    public int FixtureId { get; set; }
    public Fixture Fixture { get; set; }

    public int ClubId { get; set; }
    public Club Club { get; set; }
}