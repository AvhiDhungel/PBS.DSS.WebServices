using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace PBS.DSS.Functions
{
    public class MaintenanceFunction
    {
        [FunctionName("Function1")]
        public void Run([TimerTrigger("0 0 9 * * 0")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
