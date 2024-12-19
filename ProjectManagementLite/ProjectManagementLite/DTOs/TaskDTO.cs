// DTOs/TaskItem/TaskCreateRequest.cs
public class TaskCreateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime DueDate { get; set; }
}

// DTOs/TaskItem/TaskUpdateRequest.cs
public class TaskUpdateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime DueDate { get; set; }
}