using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AWSServerlessInvoice.Services
{
    public class AwsQueueService : IOrderQueue
    {

        private readonly string QueueUrl = "https://sqs.us-east-1.amazonaws.com/346539627952/new_orders";
        private static readonly RegionEndpoint ServiceRegion = RegionEndpoint.USEast1;
        private static IAmazonSQS client;

        public async Task AddToQueueAsync(string value)
        {
            client = new AmazonSQSClient(ServiceRegion);
            
            
            Dictionary<string, MessageAttributeValue> messageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                { "RecordId",   new MessageAttributeValue { DataType = "String", StringValue = value } }
            };

            string messageBody = $"New Order Created-{value}";

            var sendMessageRequest = new SendMessageRequest
            {
                DelaySeconds = 10,
                MessageAttributes = messageAttributes,
                MessageBody = messageBody,
                QueueUrl = QueueUrl
            };
            
            var response = await client.SendMessageAsync(sendMessageRequest);
        }
    }
}
