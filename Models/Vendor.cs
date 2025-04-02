using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OWMS.Models
{
    public class Vendor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VendorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "";

        [Required]
        [MaxLength(50)]
        public string Account { get; set; } = "";

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = "";

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
