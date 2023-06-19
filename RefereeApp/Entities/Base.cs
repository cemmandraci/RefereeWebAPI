#nullable enable
namespace RefereeApp.Entities;

public class Base
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; } = "Test";
    public DateTime? ChangedAt { get; set; }
    public string? ChangedBy { get; set; } = "Test";
    public bool IsDeleted { get; set; }
}