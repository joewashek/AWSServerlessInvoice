using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using System.Text.Json;
using Xunit;

namespace AWSServerlessInvoice.Tests
{
    public class OrderControllersTests
    {
        [Fact]
        public async Task TestPost()
        {
            var lambdaFunction = new LambdaEntryPoint();

            var requestStr = File.ReadAllText("./SampleRequests/OrderController-Post.json");
            var request = JsonSerializer.Deserialize<APIGatewayProxyRequest>(requestStr, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var context = new TestLambdaContext();
            var response = await lambdaFunction.FunctionHandlerAsync(request, context);

            Assert.Equal(200, response.StatusCode);

            //Assert.Equal("\"ORDER-123\"", response.Body);
            //Assert.True(response.MultiValueHeaders.ContainsKey("Content-Type"));
            //Assert.Equal("application/json; charset=utf-8", response.MultiValueHeaders["Content-Type"][0]);
        }
    }
}
