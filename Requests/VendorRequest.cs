namespace OWMS.Models
{
    public class VendorRequest
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string? Notes { get; set; }

        public List<BatchNumberRequest> BatchNumbers { get; set; } = new List<BatchNumberRequest>();
    }

    public class BatchNumberRequest
    {
        public string BatchCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
    }
}
