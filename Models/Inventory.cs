using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OWMS.Models
{
    public class Inventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InventoryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = "";

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "";

        [Required]
        public int Quantity { get; set; }

        [Required]
        public bool Approved { get; set; } = false;

        [Required]
        [ForeignKey("Operator")]
        public int OperatorId { get; set; }
        public User Operator { get; set; } = null!;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
