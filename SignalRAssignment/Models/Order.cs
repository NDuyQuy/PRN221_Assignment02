using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRAssignment.Models
{
    public class Order
    {
        [Key]public int OrderID { get; set; }
        [ForeignKey(nameof(Account))] public int AccountID { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public decimal? Freight { get; set; }
        [Required][MaxLength(100)] public string ShipAddress { get; set; } = string.Empty;
        public virtual Account? Account { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }


    }
}
