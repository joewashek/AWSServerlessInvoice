 using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSInvoiceFunction;

public class Function
{
    static readonly HttpClient client = new HttpClient();
    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {

    }


    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
    /// to respond to SQS messages.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        context.Logger.LogInformation($"Received message from queue");
        foreach (var message in evnt.Records)
        {
            await ProcessMessageAsync(message, context);
        }
    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        context.Logger.LogInformation($"Processing message {message.Body}");

        if(message.MessageAttributes.TryGetValue("RecordId",out var recordId))
        {
            var data = $"{{\"recordId\":\"{recordId.StringValue}\"}}";

            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            string urlOrderApi = "https://i0nfwtb65j.execute-api.us-east-1.amazonaws.com/Prod/api/order";
            var response = await client.PostAsync(urlOrderApi, content);
            response.EnsureSuccessStatusCode();

            await Task.CompletedTask;
        }
        else
        {
            context.Logger.LogInformation($"No record id attribute: {message.Body}");
            
            throw new ArgumentNullException("No RecordId Found in attributes");
        }

    }
}