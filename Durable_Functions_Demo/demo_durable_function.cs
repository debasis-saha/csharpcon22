using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Durable_Functions_Demo
{
    public static class demo_durable_function
    {
        [FunctionName("demo_durable_function")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("demo_durable_function_Hello", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("demo_durable_function_Hello", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("demo_durable_function_Hello", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("demo_durable_function_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName("demo_durable_function_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("demo_durable_function", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}