using System.Collections.Generic;
using Newtonsoft.Json;

namespace Minhdev.Api.Ecommerce.Model
{
    public class ProductResponse
    {
        [JsonProperty("items")]
        ICollection<ProductResponseItem> Items { get; set; }
    }
}