using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.SQSEvents;

namespace AWSInvoiceFunction.Tests;

public class FunctionTest
{
    [Fact]
    public async Task TestSQSEventLambdaFunction()
    {
        var attribs = new Dictionary<string, SQSEvent.MessageAttribute>();
        var att = new SQSEvent.MessageAttribute
        {
            DataType = "String",
            StringValue = "2475"
        };
        attribs.Add("RecordId",att);
        var sqsEvent = new SQSEvent
        {
            Records = new List<SQSEvent.SQSMessage>
            {
                new SQSEvent.SQSMessage
                {
                    Body = "foobar",
                    MessageAttributes = attribs 
                }
            }
        };

        var logger = new TestLambdaLogger();
        var context = new TestLambdaContext
        {
            Logger = logger
        };

        var function = new Function();
        await function.FunctionHandler(sqsEvent, context);

        Assert.Contains("Processed message foobar", logger.Buffer.ToString());
    }
}