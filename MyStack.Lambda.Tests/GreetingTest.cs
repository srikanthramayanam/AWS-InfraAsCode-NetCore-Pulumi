using Xunit;
using Amazon.Lambda.TestUtilities;

namespace MyStackLambda.Tests
{
    public class GreetingTest
    {
        [Fact]
        public void TestGreeting_With_Valid_Values()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Greeting();
            var context = new TestLambdaContext() { AwsRequestId = "Aws-Req-12345" };
            var greeting = function.GreetingHandler("cybage", context);

            Assert.Equal("HELLO CYBAGE | context AwsRequestId -" + context.AwsRequestId, greeting);
        }

        [Fact]
        public void TestGreeting_With_Null_Values()
        {
            var function = new Greeting();           
            var greeting = function.GreetingHandler("", null);
            Assert.Equal("HELLO anonynous | context is null", greeting);
        }
    }
}
