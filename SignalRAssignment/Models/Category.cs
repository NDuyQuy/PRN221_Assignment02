using System.ComponentModel.DataAnnotations;

namespace SignalRAssignment.Models
{
    public class Category
    {
        [Key]
        public int CategoryID {  get; set; }
        [Required]
        [MaxLength(70)]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(3000)]
        public string? Description { get; set; }
        public virtual List<Product>? Products { get; set; }
    }
}
