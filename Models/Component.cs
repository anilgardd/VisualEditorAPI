using System.ComponentModel.DataAnnotations.Schema;

namespace VisualEditorAPI.Models
{
    public class Component
    {
        public int Id { get; set; }
        public int DesignId { get; set; }
        public Design Design { get; set; } = null!;
        public string Type { get; set; } = string.Empty;
        public string Properties { get; set; } = string.Empty;
    }
}