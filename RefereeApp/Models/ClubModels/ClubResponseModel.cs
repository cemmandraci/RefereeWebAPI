namespace RefereeApp.Models.ClubModels;

public class ClubResponseModel
{
    public int Id { get; set; }
    public string ClubName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string ChangedBy { get; set; }
    public bool IsDeleted { get; set; }
}