using Pulumi;

namespace MyStack.Pulumi.IaC.Tests
{
    public static class OutputExtensions
    {
        public static T GetValue<T>(this Output<T> output)
        {
            T result = default;
            output.Apply(x => result = x);
            return result;
        }
    }
}
