namespace RefereeApp.Entities;

public class Referee : Base
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public int RefLevelId { get; set; }
    public int RefRegionId { get; set; }
    public RefereeLevel RefereeLevel { get; set; }
    public RefereeRegion RefereeRegion { get; set; }
    public List<Fixture> Fixtures { get; set; }
}