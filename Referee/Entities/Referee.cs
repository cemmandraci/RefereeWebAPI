namespace Referee.Entities;

public class Referee : Base
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public RefereeLevel RefereeLevelId { get; set; }
    public RefereeRegion RefereeRegionId { get; set; }
}