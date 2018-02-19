using System;

namespace ToDoList.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsDone { get; set; } = false;

        public ApplicationUser User { get; set; }
    }
}
