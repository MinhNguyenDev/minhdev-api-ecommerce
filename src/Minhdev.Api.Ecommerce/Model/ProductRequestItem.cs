using Newtonsoft.Json;

namespace Minhdev.Api.Ecommerce.Model
{
    public class ProductRequestItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}