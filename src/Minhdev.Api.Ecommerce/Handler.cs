using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Minhdev.Api.Ecommerce.Model;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Minhdev.Api.Ecommerce
{
    public class Handler
    {
        public IServiceCollection ServiceCollection { get; }

        private IResourceApiClient ResourceApiClient { get; set; }

        public Handler()
        {
            ServiceCollection = new ServiceCollection();
            ConfigureServices(ServiceCollection);
            ResolveServices();
        }

        public void ResolveServices()
        {
            var serviceProvider = ServiceCollection.BuildServiceProvider();
            ResourceApiClient = serviceProvider.GetService<IResourceApiClient>();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            ServiceCollection.AddScoped<IApplicationConfig>(x => new ApplicationConfig
            {
                ResourceEndpoint = Environment.GetEnvironmentVariable("resourceEndpoint"),
                Token = Environment.GetEnvironmentVariable("token")
            });

            ServiceCollection.AddScoped<IResourceApiClient, ResourceApiClient>();
        }

        public APIGatewayProxyResponse GetUser(
            APIGatewayProxyRequest request, ILambdaContext context)
        {
            var requestJson = JsonConvert.SerializeObject(request);
            var contextJson = JsonConvert.SerializeObject(context);
            Console.WriteLine($"Received request: {requestJson}. Context: {contextJson}");

            var nameParameter = request.QueryStringParameters != null &&
                                request.QueryStringParameters.ContainsKey("name")
                ? request.QueryStringParameters["name"]
                : null;

            if (nameParameter == null)
            {
                return CreateResponse(new UserResponse
                {
                    Name = "Minh Nguyen",
                    Token = "1234-455662-22233333-3333"
                });
            }

            if (nameParameter == "debug")
            {
                var debugResponse = new DebugResponse("Your function executed successfully!", request);

                return CreateResponse(debugResponse);
            }

            return CreateResponse(new UserResponse
            {
                Name = nameParameter,
                Token = "1234-455662-22233333-3333"
            });
        }

        public async Task<APIGatewayProxyResponse> SortProducts(
            APIGatewayProxyRequest request, ILambdaContext context)
        {
            var sortOption = request.QueryStringParameters != null &&
                             request.QueryStringParameters.ContainsKey("sortOption")
                ? request.QueryStringParameters["sortOption"]
                : null;

            if (string.IsNullOrWhiteSpace(sortOption))
            {
                return CreateErrorResponse("Must provide sort option");
            }

            var products = await ResourceApiClient.GetProductsAsync();
            var sortResult = new List<ProductResponseItem>();
            switch (sortOption.ToLower())
            {
                case "low":
                    sortResult = SortPriceAsc(products);
                    break;
                case "high":
                    sortResult = SortPriceDesc(products);
                    break;
                case "ascending":
                    sortResult = SortNameAsc(products);
                    break;
                case "descending":
                    sortResult = SortNameDesc(products);
                    break;
                case "recommended":
                    sortResult = await SortMostPopularAsync(products);
                    break;
            }

            return CreateResponse(sortResult);
        }

        private async Task<List<ProductResponseItem>> SortMostPopularAsync(ICollection<ProductResponseItem> products)
        {
            var shopperHistory = await ResourceApiClient.GetShopperHistoryAsync();
            var listOfProducts = shopperHistory.SelectMany(x => x.Products);
            var productsGroup = listOfProducts
                .GroupBy(x => x.Name)
                .OrderByDescending(g => g.Sum(item => item.Quantity));

            var result = productsGroup.Select(g => new ProductResponseItem
            {
                Name = g.Key,
                Price = g.First().Price,
                Quantity = g.Sum(i => i.Quantity)
            }).ToList();

            return result;
        }

        private List<ProductResponseItem> SortPriceAsc(
            ICollection<ProductResponseItem> products)
        {
            return products.OrderBy(x => x.Price).ToList();
        }

        private List<ProductResponseItem> SortPriceDesc(
            ICollection<ProductResponseItem> products)
        {
            return products.OrderByDescending(x => x.Price).ToList();
        }

        private List<ProductResponseItem> SortNameAsc(
            ICollection<ProductResponseItem> products)
        {
            return products.OrderBy(x => x.Name).ToList();
        }

        private List<ProductResponseItem> SortNameDesc(
            ICollection<ProductResponseItem> products)
        {
            return products.OrderByDescending(x => x.Name).ToList();
        }

        public async Task<APIGatewayProxyResponse> TrolleyTotal(
            APIGatewayProxyRequest request, ILambdaContext context)
        {
            var trolleyRequest = JsonConvert.DeserializeObject<TrolleyRequest>(request.Body);

            var result = await ResourceApiClient.CalculateTrolleyAsync(trolleyRequest);

            return new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.OK,
                Body = result.ToString(),
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"},
                    {"Access-Control-Allow-Origin", "*"}
                }
            };
        }

        private APIGatewayProxyResponse CreateErrorResponse(string msg)
        {

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int) HttpStatusCode.BadRequest,
                Body = msg
            };

            return response;
        }

        private APIGatewayProxyResponse CreateResponse<T>(T result)
        {
            int statusCode = (result != null) ? (int) HttpStatusCode.OK : (int) HttpStatusCode.InternalServerError;

            string body = (result != null) ? JsonConvert.SerializeObject(result) : string.Empty;

            var response = new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = body,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"},
                    {"Access-Control-Allow-Origin", "*"}
                }
            };

            return response;
        }
    }
}