using System.Collections.Generic;
using Newtonsoft.Json;

namespace Minhdev.Api.Ecommerce.Model
{
    public class SpecialsRequestItem
    {
        [JsonProperty("quantities")]
        public ICollection<QuantityRequestItem> Quantities { get; set; }

        [JsonProperty("total")]
        public decimal Total { get; set; }
    }
}