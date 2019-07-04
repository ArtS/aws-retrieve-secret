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
            // Use the following form to take credentials directly from `AWS_ACCESS_KEY_ID` and `AWS_SECRET_ACCESS_KEY`
            // environment variables:
            // var client = new AmazonSecretsManagerClient(RegionEndpoint.APSoutheast2);

            var client = new AmazonSecretsManagerClient(accessKeyId, secretAccessKey, RegionEndpoint.APSoutheast2);

            var request = new GetSecretValueRequest
            {
                // this gets your secret name, 'web-api/passwords/database' in our case
                SecretId = secretName
            };

            GetSecretValueResponse response = null;

            try
            {
                response = client.GetSecretValueAsync(request).Result;
            }
            //
            // Exceptions are taken from AWS SDK API Reference here:
            // https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html#API_GetSecretValue_Errors
            //
            // Setting breakpoints inside every exception handler can help you identify what's
            // wrong in each individual situation.
            //
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
