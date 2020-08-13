using System.Threading.Tasks;
using Pulumi;

namespace MyStack.Iac
{
    class Program
    {
        static Task<int> Main() => Deployment.RunAsync<S3LambdaStack>();
    }
}
