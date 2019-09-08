using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Minhdev.Api.Ecommerce.Model;

namespace Minhdev.Api.Ecommerce
{
    public interface IResourceApiClient
    {
        Task<ICollection<ProductResponseItem>> GetProductsAsync();
        Task<ICollection<ShopperHistoryResponseItem>> GetShopperHistoryAsync();
        Task<decimal> CalculateTrolleyAsync(TrolleyRequest request);
    }

    public class ResourceApiClient : RestApiClient, IResourceApiClient
    {
        private readonly IApplicationConfig _config;
        private const string ProductPath = "products";
        private const string ShopperHistoryPath = "shopperHistory";
        private const string TrolleyCalculatorPath = "trolleyCalculator";

        public ResourceApiClient(IApplicationConfig config) : base(config.ResourceEndpoint)
        {
            _config = config;
        }

        public async Task<ICollection<ProductResponseItem>> GetProductsAsync()
        {
            var request = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("token", _config.Token)
            };

            return await GetAsync<ICollection<ProductResponseItem>>(request, ProductPath);
        }

        public async Task<ICollection<ShopperHistoryResponseItem>> GetShopperHistoryAsync()
        {
            var request = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("token", _config.Token)
            };

            return await GetAsync<ICollection<ShopperHistoryResponseItem>>(request, ShopperHistoryPath);
        }

        public async Task<decimal> CalculateTrolleyAsync(TrolleyRequest request)
        {
            var requestParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("token", _config.Token)
            };

            var result = await PostAsync(requestParams, request, TrolleyCalculatorPath);
            return decimal.Parse(result);
        }
    }
}