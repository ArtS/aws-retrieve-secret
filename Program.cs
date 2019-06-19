using System;
using System.Runtime.InteropServices;

namespace aws_retrieve_secret
{
    static class Program
    {
        static void Main(string[] args)
        {
            var awsAccessKeyIdEnvName = "AWS_ACCESS_KEY_ID";
            var awsSecretAccessKeyEnvName = "AWS_SECRET_ACCESS_KEY";
            
            var accessKeyId = Environment.GetEnvironmentVariable(awsAccessKeyIdEnvName);
            var secretAccessKey = Environment.GetEnvironmentVariable(awsSecretAccessKeyEnvName);

            if (string.IsNullOrWhiteSpace(accessKeyId))
            {
                Console.WriteLine($"Looks like your {awsAccessKeyIdEnvName} environment variable is not set.");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(secretAccessKey))
            {
                Console.WriteLine($"Looks like your {awsSecretAccessKeyEnvName} environment variable is not set.");
                return;
            }
            
            var sr = new SecretsReader(accessKeyId, secretAccessKey);

            Console.WriteLine($"\nRetrieving secret value from AWS Secrets Manager...\n");
            var secret = sr.GetSecret("myDbPassword");
            Console.WriteLine($"Secret value obtained from AWS Secrets Manager is: {secret}\n");
            
        }
    }
}