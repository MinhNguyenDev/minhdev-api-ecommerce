using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Minhdev.Api.Ecommerce.Model;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Minhdev.Api.Ecommerce.UnitTests
{
    public class HandlerSortProductsTests
    {
        private const long MaxPrice = 999999999999;
        private const int MinPrice = 1;

        private const string FirstProductName = "Test Product A";
        private const string LastProductName = "Test Product F";
        private const string MostPurchasedProductName = "Test Product B";
        private const string LessPurchasedProductName = "Test Product F";

        private Handler _handler;
        private APIGatewayProxyRequest _request;

        public HandlerSortProductsTests()
        {
            Environment.SetEnvironmentVariable("resourceEndpoint", "http://test");
            _handler = new Handler();
            var mockClient = new Mock<IResourceApiClient>();
            mockClient.Setup(x => x.GetProductsAsync()).ReturnsAsync(
                new List<ProductResponseItem>
                {
                    new ProductResponseItem {Name = "Test Product A", Price = 999999999999, Quantity = 1},
                    new ProductResponseItem {Name = "Test Product F", Price = 1, Quantity = 1},
                }
            );

            mockClient.Setup(x => x.GetShopperHistoryAsync()).ReturnsAsync(
                new List<ShopperHistoryResponseItem>
                {
                    new ShopperHistoryResponseItem
                    {
                        CustomerId = 1,
                        Products = new List<ProductResponseItem>
                        {
                            new ProductResponseItem {Name = "Test Product A", Price = 999999999999, Quantity = 1},
                        }
                    },
                    new ShopperHistoryResponseItem
                    {
                        CustomerId = 2,
                        Products = new List<ProductResponseItem>
                        {
                            new ProductResponseItem {Name = "Test Product F", Price = 999999999999, Quantity = 1},
                            new ProductResponseItem {Name = "Test Product B", Price = 5, Quantity = 10}
                        }
                    },
                    new ShopperHistoryResponseItem
                    {
                        CustomerId = 3,
                        Products = new List<ProductResponseItem>
                        {
                            new ProductResponseItem {Name = "Test Product A", Price = 999999999999, Quantity = 1},
                            new ProductResponseItem {Name = "Test Product B", Price = 5, Quantity = 10}
                        }
                    }
                }
            );

            _handler.ServiceCollection.AddScoped(x => mockClient.Object);
            _handler.ResolveServices();
            _request = new APIGatewayProxyRequest
            {
                QueryStringParameters = new Dictionary<string, string>()
            };
        }

        [Fact]
        public async Task SortProducts_SortOptionLow_ReturnLowToHighPrice()
        {
            _request.QueryStringParameters["sortOption"] = "Low";
            var response = await _handler.SortProducts(_request, null);

            var sortProducts = JsonConvert.DeserializeObject<ICollection<ProductResponseItem>>(response.Body);
            sortProducts.First().Price.Should().Be(MinPrice);
            sortProducts.Last().Price.Should().Be(MaxPrice);
        }

        [Fact]
        public async Task SortProducts_SortOptionHigh_ReturnHighToLowPrice()
        {
            _request.QueryStringParameters["sortOption"] = "High";
            var response = await _handler.SortProducts(_request, null);

            var sortProducts = JsonConvert.DeserializeObject<ICollection<ProductResponseItem>>(response.Body);
            sortProducts.First().Price.Should().Be(MaxPrice);
            sortProducts.Last().Price.Should().Be(MinPrice);
        }

        [Fact]
        public async Task SortProducts_SortOptionAscending_ReturnAscendingName()
        {
            _request.QueryStringParameters["sortOption"] = "Ascending";
            var response = await _handler.SortProducts(_request, null);

            var sortProducts = JsonConvert.DeserializeObject<ICollection<ProductResponseItem>>(response.Body);
            sortProducts.First().Name.Should().Be(FirstProductName);
            sortProducts.Last().Name.Should().Be(LastProductName);
        }

        [Fact]
        public async Task SortProducts_SortOptionDescending_ReturnDescendingName()
        {
            _request.QueryStringParameters["sortOption"] = "Descending";
            var response = await _handler.SortProducts(_request, null);

            var sortProducts = JsonConvert.DeserializeObject<ICollection<ProductResponseItem>>(response.Body);
            sortProducts.First().Name.Should().Be(LastProductName);
            sortProducts.Last().Name.Should().Be(FirstProductName);
        }

        [Fact]
        public async Task SortProducts_SortOptionRecommended_ReturnMostPurchasedProducts()
        {
            // Assume that sort based on popularity means most purchased product will display first
            _request.QueryStringParameters["sortOption"] = "Recommended";
            var response = await _handler.SortProducts(_request, null);

            var sortProducts = JsonConvert.DeserializeObject<ICollection<ProductResponseItem>>(response.Body);
            sortProducts.First().Name.Should().Be(MostPurchasedProductName);
            sortProducts.First().Quantity.Should().Be(20);
            sortProducts.Last().Name.Should().Be(LessPurchasedProductName);
            sortProducts.Last().Quantity.Should().Be(1);
        }
    }
}
