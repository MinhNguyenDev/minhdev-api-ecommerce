using Newtonsoft.Json;

namespace Minhdev.Api.Ecommerce.Model
{
    public class QuantityRequestItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}