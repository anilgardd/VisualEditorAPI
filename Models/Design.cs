using System;
using System.Collections.Generic;

namespace VisualEditorAPI.Models
{
    public class Design
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Component> Components { get; set; } = new List<Component>();
    }
}