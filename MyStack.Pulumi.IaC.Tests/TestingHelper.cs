using System.Collections.Immutable;
using System.Threading.Tasks;
using Moq;
using Pulumi;
using Pulumi.Testing;

namespace MyStack.IaC.Tests
{
    public static class TestingHelper
    {
        /// <summary>
        /// Run the tests for a given stack type.
        /// </summary>
        public static Task<ImmutableArray<Resource>> RunAsync<T>() where T : Stack, new()
        {
            var mocks = new Mock<IMocks>();

            mocks.Setup(m => m.NewResourceAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<ImmutableDictionary<string, object>>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string type, string name, ImmutableDictionary<string, object> inputs, string provider,
                string id) =>
                {
                    var outputs = ImmutableDictionary.CreateBuilder<string, object>();
                    outputs.AddRange(inputs);

                    if (!inputs.ContainsKey("name"))
                        outputs.Add("name", name);

                    // For a Storage Account...
                    if (type == "aws:lambda/function:Function")
                    {
                        // ... set its web endpoint property.
                        // Normally this would be calculated by AWS, so we have to mock it. 
                        outputs.Add("Arn", $"arn:aws:lambda:ap-south-1:652183896769:function:{name}");
                    }

                    id ??= $"{name}_id";
                    return (name + "_id", outputs);
                });
                //.ReturnsAsync(NewResourceAsync());

            //mocks.Setup(m => m.CallAsync(It.IsAny<string>(), 
            //    It.IsAny<ImmutableDictionary<string, object>>(), It.IsAny<string>()))
            //    .ReturnsAsync((string token, ImmutableDictionary<string, object> args, string provider) => args);

            return Deployment.TestAsync<T>(mocks.Object, new TestOptions { IsPreview = false });
        }

        public static Task<(string id, object state)> NewResourceAsync(string type, string name, ImmutableDictionary<string, object> inputs, string? provider, string? id)
        {
            var outputs = ImmutableDictionary.CreateBuilder<string, object>();

            // Forward all input parameters as resource outputs, so that we could test them.
            outputs.AddRange(inputs);

            // Set the name to resource name if it's not set explicitly in inputs.
            if (!inputs.ContainsKey("name"))
                outputs.Add("name", name);

            if (type == "azure:storage/blob:Blob")
            {
                // Assets can't directly go through the engine.
                // We don't need them in the test, so blank out the property for now.
                outputs.Remove("source");
            }

            // For a Storage Account...
            if (type == "azure:storage/account:Account")
            {
                // ... set its web endpoint property.
                // Normally this would be calculated by Azure, so we have to mock it. 
                outputs.Add("primaryWebEndpoint", $"https://{name}.web.core.windows.net");
            }

            // Default the resource ID to `{name}_id`.
            // We could also format it as `/subscription/abc/resourceGroups/xyz/...` if that was important for tests.
            id ??= $"{name}_id";
            return Task.FromResult((id, (object)outputs));
        }

        /// <summary>
        /// Extract the value from an output.
        /// </summary>
        public static Task<T> GetValueAsync<T>(this Output<T> output)
        {
            var tcs = new TaskCompletionSource<T>();
            output.Apply(v =>
            {
                tcs.SetResult(v);
                return v;
            });
            return tcs.Task;
        }
    }
}
