using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Durable_Functions_Demo
{
    public static class demo_fan_in_fan_out_2
    {
        [FunctionName("demo_fan_in_fan_out_2")]
        public static async Task<int> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var _parallel_Tasks = new List<Task<int>>();
            int _final_results = 0;

            //Get a list of Number to Process it in Parallel
            int[] _batch_data = await context.CallActivityAsync<int[]>("F1", null);
            foreach (var item in _batch_data)
            {
                Task<int> _task_value = context.CallActivityAsync<int>("F2", item);
                _parallel_Tasks.Add(_task_value);
            }

            await Task.WhenAll(_parallel_Tasks);

            // aggregate all the numbers and send result to F3
            int _sum_value = _parallel_Tasks.Sum(x => x.Result);
            _final_results = await context.CallActivityAsync<int>("F3", _sum_value);
            return _final_results;
        }

        [FunctionName("F1")]
        public static int[] F1([ActivityTrigger] string name, ILogger log)
        {
            return new int[] { 5, 10, 15, 20 };
        }

        [FunctionName("F2")]
        public static async Task<int> F2([ActivityTrigger] int value, ILogger log)
        {
            return await Task.FromResult(value);
        }
        [FunctionName("F3")]
        public static int F3([ActivityTrigger] int value, ILogger log)
        {
            return value * 2;
        }
    }
}