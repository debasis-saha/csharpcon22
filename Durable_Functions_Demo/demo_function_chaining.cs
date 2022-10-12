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
    public static class demo_function_chaining
    {
        [FunctionName("function_chaining_demo")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            var _input_param = new InputParams()
            {
                EventName = "CSharp Confererence 2022",
                PersonName = "Debasis Saha"
            };

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("function_chaining_demo_Hello", "New Delhi"));
            outputs.Add(await context.CallActivityAsync<string>("function_chaining_demo_Hello", "Mumbai"));
            outputs.Add(await context.CallActivityAsync<string>("function_chaining_demo_Hello", "Chennai"));
            outputs.Add(await context.CallActivityAsync<string>("function_chaining_demo_Hello_User", _input_param));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("function_chaining_demo_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName("function_chaining_demo_Hello_User")]
        public static string SayHello2([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            var _input_param = context.GetInput<InputParams>();
            log.LogInformation($"Thanks {_input_param.PersonName} for participating at the {_input_param.EventName} event.");
            return $"Thanks {_input_param.PersonName} for participating at the {_input_param.EventName} event.";
        }         
    }
}