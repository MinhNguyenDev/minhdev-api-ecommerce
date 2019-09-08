using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using FluentAssertions;
using Minhdev.Api.Ecommerce.Model;
using Newtonsoft.Json;
using Xunit;

namespace Minhdev.Api.Ecommerce.UnitTests
{
    public class HandlerGetUserTests
    {
        private Handler _handler;
        private APIGatewayProxyRequest _request;

        public HandlerGetUserTests()
        {
            Environment.SetEnvironmentVariable("resourceEndpoint", "http://test");
            _handler = new Handler();
            _request = new APIGatewayProxyRequest
            {
                QueryStringParameters = new Dictionary<string, string>()
            };
        }

        [Fact]
        public void GetUser_NameParamIsEmpty_ReturnTestData()
        {
            var response = _handler.GetUser(_request, null);

            var responseUser = JsonConvert.DeserializeObject<UserResponse>(response.Body);
            responseUser.Name.Should().Be("Minh Nguyen");
            responseUser.Token.Should().Be("1234-455662-22233333-3333");
        }

        [Fact]
        public void GetUser_NameParamHasValue_ReturnName()
        {
            _request.QueryStringParameters["name"] = "John";
            var response = _handler.GetUser(_request, null);

            var responseUser = JsonConvert.DeserializeObject<UserResponse>(response.Body);
            responseUser.Name.Should().Be("John");
        }

        [Fact]
        public void GetUser_NameParamIsDebug_ReturnDebugData()
        {
            _request.QueryStringParameters["name"] = "debug";
            var response = _handler.GetUser(_request, null);

            var debugResponse = JsonConvert.DeserializeObject<DebugResponse>(response.Body);
            debugResponse.Message.Should().NotBeEmpty();
        }
    }
}
