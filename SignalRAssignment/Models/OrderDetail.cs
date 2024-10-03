using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRAssignment.Models
{
    public class OrderDetail
    {
        [ForeignKey(nameof(Order))] public int OrderID {  get; set; }
        [ForeignKey(nameof(Product))] public int ProductID { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
    }
}
