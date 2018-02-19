using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ToDoList.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<TodoItem> TodoItems { get; set; }
    }
}
