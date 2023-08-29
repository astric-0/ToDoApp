using Server.Utils;

namespace Server.Models;
public class Tasks
{
    public int Id { get; set; }
    public string TaskName { get; set; } = null!;
    public string? TaskDetails { get; set; } = null;
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public DateTime? ModifiedOn { get; set; } = null;
    public DateTime? CompletedOn { get; set; } = null;
    public DateTime? Deadline { get; set; } = null;
    public int Importance { get; set; } = 2;
    public string Category { get; set; } = "Home";
    public bool IsCompleted { get; set; } = false;

    public string? DeadlineDateString { get; set; } = null;

    public bool IsValidOrException ()
    {
        if (string.IsNullOrEmpty(this.TaskName))
            throw new StatusException(400, "TaskName can't be empty");
        else if (string.IsNullOrEmpty(this.Category) || (this.Category != "HOME" && this.Category != "OFFICE" && this.Category != "MARKET"))
            throw new StatusException(400, "Category is empty or invalid");
        else if (this.Importance < 1 || this.Importance > 3)
            throw new StatusException(400, "Importance is invalid");

        if (!string.IsNullOrEmpty(this.DeadlineDateString))
            this.Deadline = Convert.ToDateTime(this.DeadlineDateString);
        else 
            this.Deadline = null;

        return true;
    }
}