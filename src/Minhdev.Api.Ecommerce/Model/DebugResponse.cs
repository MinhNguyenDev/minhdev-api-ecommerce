using Amazon.Lambda.APIGatewayEvents;

namespace Minhdev.Api.Ecommerce.Model
{
    public class DebugResponse
    {
        public string Message {get; set;}
        public APIGatewayProxyRequest Request {get; set;}

        public DebugResponse(string message, APIGatewayProxyRequest request){
            Message = message;
            Request = request;
        }
    }
}