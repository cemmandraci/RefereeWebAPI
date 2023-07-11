namespace RefereeApp.Entities;

public class Referee : Base
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public Guid UserId { get; set; }
    public int RefLevelId { get; set; }
    public int RefRegionId { get; set; }
    public RefereeLevel RefereeLevel { get; set; }
    public RefereeRegion RefereeRegion { get; set; }
    public virtual List<Fixture> Fixtures { get; set; }
    public User Users { get; set; }
}