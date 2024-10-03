using System.ComponentModel.DataAnnotations;

namespace SignalRAssignment.Models
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string HashPassword { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Range(1, 2)]
        public byte Type { get; set; } = 2;

        [MaxLength(100)]
        public string? ContactName {  get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        [MaxLength(15)]
        public string? Phone { get; set; }

        public virtual ICollection<Order>? Orders { get; set; }
    }
}
