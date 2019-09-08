using Newtonsoft.Json;

namespace Minhdev.Api.Ecommerce.Model
{
    public class UserResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}