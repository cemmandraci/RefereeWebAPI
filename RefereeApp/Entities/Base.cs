namespace RefereeApp.Entities;

public class Base
{
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; }
}