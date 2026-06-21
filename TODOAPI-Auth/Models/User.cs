namespace TODOAPI_Auth.Models
{
    public class User
    {
        public int Id { get; set; }
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; }  = string.Empty ;
        public string email { get; set; } = string .Empty ;
        public string passwordHash { get; set; } = string.Empty;
        public DateTime createdAt { get; set; } = DateTime.Now;


        //One to Many: One user has many TODos
        public List<TodoItem> Todos { get; set; } = new List<TodoItem>(); //Foreign key
    }
}
