using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OWMS.Models
{
    public enum UserRole
    {
        Admin,
        User
    }

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = "";

        [Required]
        [MaxLength(200)]
        public string Password { get; set; } = "";

        [Required]
        public UserRole Role { get; set; } = UserRole.Admin;

        public ICollection<Inventory> InventoryLogs { get; set; } = new List<Inventory>();
    }
}
