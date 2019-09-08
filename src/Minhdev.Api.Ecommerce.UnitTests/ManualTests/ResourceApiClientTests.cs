using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Minhdev.Api.Ecommerce.Model;
using Xunit;

namespace Minhdev.Api.Ecommerce.UnitTests
{
    public class ResourceApiClientTests
    {
        [Fact(Skip = "Manual test")]
        public async Task GetProducts_Should_Return_Ok()
        {
            var client = new ResourceApiClient(new ApplicationConfig
            {
                ResourceEndpoint = "http://resource-endpoint/api/resource/",
                Token = "Enter token here"
            });

            var result = await client.GetProductsAsync();

            result.Should().NotBeEmpty();
        }

        [Fact(Skip = "Manual test")]
        public async Task GetShopperHistory_Should_Return_Ok()
        {
            var client = new ResourceApiClient(new ApplicationConfig
            {
                ResourceEndpoint = "http://resource-endpoint/api/resource/",
                Token = "Enter token here"
            });

            var result = await client.GetShopperHistoryAsync();

            result.Should().NotBeEmpty();
        }

        [Fact(Skip = "Manual test")]
        public async Task CalculateTrolley_Should_Return_Ok()
        {
            var client = new ResourceApiClient(new ApplicationConfig
            {
                ResourceEndpoint = "http://resource-endpoint/api/resource/",
                Token = "Enter token here"
            });

            var request = new TrolleyRequest
            {
                Products = new List<ProductRequestItem>()
                {
                    new ProductRequestItem() {Name = "Test Product B", Price = 101.99M}
                },
                Specials = new List<SpecialsRequestItem>()
                {
                    new SpecialsRequestItem()
                    {
                        Quantities = new List<QuantityRequestItem>()
                        {
                            new QuantityRequestItem() {Name = "Test Product B", Quantity = 1}
                        },
                        Total = 1
                    }
                },
                Quantities = new List<QuantityRequestItem>()
                {
                    new QuantityRequestItem() {Name = "Test Product B", Quantity = 1}
                }
            };

            var result = await client.CalculateTrolleyAsync(request);

        }
    }
}
