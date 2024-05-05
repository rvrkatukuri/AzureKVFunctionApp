using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.Collections.Generic;

namespace FunctionApp
{
    public static class AzureKvFunction
    {
        [FunctionName("KeyVaultRetrieval")]
        public static async Task<ActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var secretClient = new SecretClient(new Uri($"https://keyvault0419.vault.azure.net/"), new DefaultAzureCredential());
            var secrets = new List<string>();

            await foreach (var secretProperties in secretClient.GetPropertiesOfSecretsAsync())
            {
                try
                {
                    KeyVaultSecret secret = await secretClient.GetSecretAsync(secretProperties.Name);
                    secrets.Add(secret.Value);
                }
                catch (Exception ex)
                {
                    return new StatusCodeResult(500);
                }
            }

            return new JsonResult(secrets);

        }
    }
}
