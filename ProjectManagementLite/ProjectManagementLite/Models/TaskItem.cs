namespace ProjectManagementLite.Models;

public class TaskItem
{
    public int Id { get; set; }
    public int ProjectId { get; set; } // Foreign key to Project
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "Unassigned"; // e.g., Pending, In Progress, Completed
    public DateTime DueDate { get; set; }
}