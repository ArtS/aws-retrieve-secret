using System;
using System.IO;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace aws_retrieve_secret
{
    public class SecretsReader
    {
        private readonly string accessKeyId;
        private readonly string secretAccessKey;
        public SecretsReader(string accessKeyId, string secretAccessKey)
        {
            this.accessKeyId = accessKeyId;
            this.secretAccessKey = secretAccessKey;
        }
        
        public string GetSecret(string secretName)
        {
            IAmazonSecretsManager client = new AmazonSecretsManagerClient(accessKeyId, secretAccessKey, RegionEndpoint.APSoutheast2);

            var request = new GetSecretValueRequest
            {
                SecretId = secretName
            };
            
            GetSecretValueResponse response = null;
            
            // Exceptions are taken from AWS SDK API Reference here:
            // https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html#API_GetSecretValue_Errors
            // 
            // Setting breakpoints inside every exception handler can help you identify what's 
            // wrong in each individual situation. 
            try
            {
                response = client.GetSecretValueAsync(request).Result;
            }
            catch (DecryptionFailureException e)
            {
                // Secrets Manager can't decrypt the protected secret text using the provided KMS key.
                throw;
            }
            catch (InternalServiceErrorException e)
            {
                // An error occurred on the server side.
                throw;
            }
            catch (InvalidParameterException e)
            {
                // You provided an invalid value for a parameter.
                throw;
            }
            catch (InvalidRequestException e)
            {
                // You provided a parameter value that is not valid for the current state of the resource.
                throw;
            }
            catch (ResourceNotFoundException e)
            {
                // We can't find the resource that you asked for.
                throw;
            }
            catch (System.AggregateException ae)
            {
                // More than one of the above exceptions were triggered.
                throw;
            }

            if (response.SecretString != null)
            {
                return response.SecretString;
            }

            // If secret is stored as a binary string, it needs to be decoded
            using (var memoryStream = response.SecretBinary)
            {
                using (var reader = new StreamReader(memoryStream))
                {
                    return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
                }
            }
        }
    }
}