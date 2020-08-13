using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Pulumi.Aws.S3;
using MyStack.Iac;
using Pulumi.Aws.Lambda;

namespace MyStack.IaC.Tests
{
    public class MyStackTest
    {
        
        [SetUp]
        public void Setup()
        {
           
        }

        [Test]
        public async Task S3LambdaStack_S3_Lambda_Should_be_Created()
        {
            var resources = await TestingHelper.RunAsync<S3LambdaStack>();
            var bucket = resources.OfType<Bucket>().FirstOrDefault();
            bucket.BucketName.GetValueAsync().Should().NotBeNull("rdstack-bucket");
            var lambda = resources.OfType<Function>().FirstOrDefault();
            lambda.Name.GetValueAsync().Should().NotBeNull("rdstackgreeting");
        }
    }
}