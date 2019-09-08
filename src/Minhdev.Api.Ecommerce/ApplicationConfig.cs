namespace Minhdev.Api.Ecommerce
{
    public interface IApplicationConfig
    {
        string ResourceEndpoint { get; set; }
        string Token { get; set; }
    }

    public class ApplicationConfig : IApplicationConfig
    {
        public string ResourceEndpoint { get; set; }

        public string Token { get; set; }
    }
}