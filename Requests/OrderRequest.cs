using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OWMS.Requests
{
    public class OrderRequest
    {
        [JsonPropertyName("orderIds")]
        public List<int> OrderIds { get; set; } = new List<int>();
        [JsonPropertyName("batchCode")]
        public string BatchCode { get; set; } = string.Empty;

    }
}
