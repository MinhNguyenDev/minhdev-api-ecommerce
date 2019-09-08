using System.Collections.Generic;
using Newtonsoft.Json;

namespace Minhdev.Api.Ecommerce.Model
{
    public class ShopperHistoryResponseItem
    {
        [JsonProperty("customerId")]
        public long CustomerId { get; set; }

        [JsonProperty("products")]
        public ICollection<ProductResponseItem> Products { get; set; }
    }
}