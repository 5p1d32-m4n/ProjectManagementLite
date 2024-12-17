namespace ProjectManagementLite.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; } // Foreign key to User
}