namespace TODOAPI_Auth.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "Medium";


        // Foreign Key — links todo to its owner
        public int UserId { get; set; }
        public User User { get; set; } = null!;

    }
}
