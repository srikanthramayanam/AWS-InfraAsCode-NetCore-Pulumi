using Pulumi;
using Pulumi.Aws.Kms;
using Pulumi.Aws.S3;
using Pulumi.Aws.Lambda;
using Pulumi.Aws.Iam;

class MyStack : Stack
{
    private readonly string _logstackBucketName = "rdstack-bucket";
    private readonly string _logstackKmsKeyName = "rdstack-kms-key";
    private readonly string _logStackLambdaRoleName = "rdstackLambdaRole";
    private readonly string _logstackLambda = "rdstackgreeting";

    public MyStack()
    {
        var key = new Key(_logstackKmsKeyName);

        // Create an AWS resource (S3 Bucket)
        var bucket = CreateOrUpdateS3Bucket(_logstackBucketName, key);

        // Export the name of the bucket
        this.BucketName = bucket.Id;

        var lambda = CreateOrUpdateLambda(_logstackLambda);

        // Export the arn of the Lambda
        RDStackLambda = lambda.Arn;
    }

    [Output]
    public Output<string> BucketName { get; set; }

    [Output] 
    public Output<string> RDStackLambda { get; set; }

    private Bucket CreateOrUpdateS3Bucket(string bucketName, Key key)
    {
        // Create an AWS resource (S3 Bucket)
        var bucket = new Bucket(bucketName, new BucketArgs
        {
            ServerSideEncryptionConfiguration = new Pulumi.Aws.S3.Inputs.BucketServerSideEncryptionConfigurationArgs
            {
                Rule = new Pulumi.Aws.S3.Inputs.BucketServerSideEncryptionConfigurationRuleArgs
                {
                    ApplyServerSideEncryptionByDefault = new Pulumi.Aws.S3.Inputs.BucketServerSideEncryptionConfigurationRuleApplyServerSideEncryptionByDefaultArgs
                    {
                        SseAlgorithm = "aws:kms",
                        KmsMasterKeyId = key.Id,
                    },
                },
            },
        });
        return bucket;
    }

    private Function CreateOrUpdateLambda(string name)
    {
        var lambda = new Function(name, new FunctionArgs
        {
            Runtime = "dotnetcore3.1",
            Code = new FileArchive("../MyStack.Lambda/bin/Release/netcoreapp3.1/publish"),
            Handler = "MyStack.Lambda::MyStackLambda.Greeting::GreetingHandler",
            Role = CreateLambdaRole().Arn
        });
        return lambda;
    }
    private Role CreateLambdaRole()
    {
        var lambdaRole = new Role(_logStackLambdaRoleName, new RoleArgs
        {
            AssumeRolePolicy = "{\"Version\":\"2012-10-17\",\"Statement\":[{\"Action\":\"sts:AssumeRole\",\"Principal\":{\"Service\":\"lambda.amazonaws.com\"},\"Effect\":\"Allow\",\"Sid\":\"\"}]}"
            //    @"{
            //    ""Version"": ""2020-08-11"",
            //    ""Statement"": [
            //        {
            //            ""Action"": ""sts:AssumeRole"",
            //            ""Principal"": {
            //                ""Service"": ""lambda.amazonaws.com""
            //            },
            //            ""Effect"": ""Allow"",
            //            ""Sid"": """"
            //        }
            //    ]
            //}"            

        });

        var logPolicy = new RolePolicy("lambdaLogPolicy", new RolePolicyArgs
        {
            Role = lambdaRole.Id,
            Policy = "{\"Version\":\"2012-10-17\",\"Statement\":[{\"Effect\":\"Allow\",\"Action\":[\"logs:CreateLogGroup\",\"logs:CreateLogStream\",\"logs:PutLogEvents\"],\"Resource\":\"arn:aws:logs:*:*:*\"}]}"
            //    @"{
            //    ""Version"": ""2020-08-11"",
            //    ""Statement"": [{
            //        ""Effect"": ""Allow"",
            //        ""Action"": [
            //            ""logs:CreateLogGroup"",
            //            ""logs:CreateLogStream"",
            //            ""logs:PutLogEvents""
            //        ],
            //        ""Resource"": ""arn:aws:logs:*:*:*""
            //    }]
            //}"
        });

        return lambdaRole;
    }
}
