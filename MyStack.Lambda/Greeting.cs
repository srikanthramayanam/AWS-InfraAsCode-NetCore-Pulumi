using System.Text;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MyStackLambda
{
    public class Greeting
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GreetingHandler(string name, ILambdaContext context)
        {
            var greeting = new StringBuilder("HELLO ");
            if (!string.IsNullOrWhiteSpace(name))
            {
                greeting.Append(name.ToUpper());
            }
            else {
                greeting.Append("anonynous");
            }

            greeting.Append(" | ");

            if (context != null)
            {                
                greeting.Append("context AwsRequestId -" + context.AwsRequestId);
            }
            else
            {
                greeting.Append("context is null");
            }

            return greeting.ToString();
        }
    }
}
