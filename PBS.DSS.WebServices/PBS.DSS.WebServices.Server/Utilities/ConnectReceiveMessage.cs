using Microsoft.AspNetCore.Mvc;

namespace PBS.DSS.WebServices.Server.Utilities
{
    public class ConnectReceiveMessage<T>(T o)
    {
        public T Object { get; set; } = o;
        public bool HasCompleted { get; set; } = false;
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;

        public ActionResult<T> GetResult()
        {
            return HasError ? new ObjectResult(ErrorMessage) { StatusCode = 500 } : Object;
        }
    }
}
