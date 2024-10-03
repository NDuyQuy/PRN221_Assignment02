using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRAssignment.Models
{
    public class Product
    {
        [Key]public int ProductID { get; set; }
        [Required][MaxLength(50)] public string ProductName { get; set; } = string.Empty;
        [Required][ForeignKey(nameof(Supplier))] public int SupplierID { get; set; }
        [Required][ForeignKey(nameof(Category))] public int CategoryID { get; set; }
        [Required] public int Quantity { get; set; }
        [Required] public Decimal Price {  get; set; }
        [Required] public string ProductImage { get; set; } = string.Empty;

        public virtual Supplier? Supplier { get; set; }
        public virtual Category? Category { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }

    }
}
