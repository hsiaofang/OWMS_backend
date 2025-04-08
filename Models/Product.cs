using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OWMS.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = "";

        [Required]
        public int Price { get; set; }

        [MaxLength(500)]
        public string? PhotoUrl { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public string QRCode { get; set; } = "";

        [Required]
        [ForeignKey("Vendor")]
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; } = null!;

        [Required]
        [ForeignKey("Counter")]
        public int CounterId { get; set; }
        public Counter Counter { get; set; } = null!;

        public ICollection<Inventory> Inventorys { get; set; } = new List<Inventory>();
    }
}
