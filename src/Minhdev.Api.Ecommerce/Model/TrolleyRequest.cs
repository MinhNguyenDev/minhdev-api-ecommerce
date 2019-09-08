using System.Collections.Generic;
using Newtonsoft.Json;

namespace Minhdev.Api.Ecommerce.Model
{
    public class TrolleyRequest
    {
        [JsonProperty("products")]
        public ICollection<ProductRequestItem> Products { get; set; }

        [JsonProperty("specials")]
        public ICollection<SpecialsRequestItem> Specials { get; set; }

        [JsonProperty("quantities")]
        public ICollection<QuantityRequestItem> Quantities { get; set; }
    }
}