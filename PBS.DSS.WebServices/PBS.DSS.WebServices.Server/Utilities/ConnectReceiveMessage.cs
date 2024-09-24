using Microsoft.AspNetCore.Mvc;

namespace PBS.DSS.WebServices.Server.Utilities
{
    public class ConnectReceiveMessage<T>(T o)
    {
        private static readonly int TimeOut = 2 * 60 * 1000;

        public T Object { get; set; } = o;
        public bool HasCompleted { get; set; } = false;
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;

        public ActionResult<T> GetResult()
        {
            return HasError ? new ObjectResult(ErrorMessage) { StatusCode = 500 } : Object;
        }

        public void WaitForCompletion() => WaitForCompletion(TimeOut);
        public void WaitForCompletion(int timeoutInMS)
        {
            int waited = 0;

            while (!HasCompleted)
            {
                Thread.Sleep(500);
                waited += 500;

                if (waited > timeoutInMS)
                {
                    HasError = true;
                    ErrorMessage = "Connection is taking too long to respond.";
                    HasCompleted = true;
                }
            }
        }
    }
}
