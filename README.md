# AWS-InfraAsCode-NetCore-Pulumi
.Net Core application to create AWS Infra as code (IaC) using Pulumi

# Before You Begin
1. Install Pulumi as per documentation @ https://www.pulumi.com/docs/get-started/aws/begin/
2. Build the application
3. goto AWS-InfraAsCode-NetCore-Pulumi\MyStack.Pulumi.IaC
4. open cmd
5. to build the application and deploy stack - run command  pulumi up
6. if all looks ok, then it will create S3 bucket and Lambda function in given region
7. to destroy stack from AWS - run command pulumi destroy

More deatils @ https://www.pulumi.com/docs/get-started/aws/begin/
