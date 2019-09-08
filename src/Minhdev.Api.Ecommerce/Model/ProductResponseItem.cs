using Newtonsoft.Json;

namespace Minhdev.Api.Ecommerce.Model
{
    public class ProductResponseItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }
    }
}