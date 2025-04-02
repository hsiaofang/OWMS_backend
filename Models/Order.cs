using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OWMS.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey("IntervalNumber")]
        public int IntervalNumberId { get; set; }

        public IntervalNumber IntervalNumber { get; set; } = null!;

        [ForeignKey("Vendor")]
        public int VendorId { get; set; }

        public Vendor Vendor { get; set; } = null!;
    }
}