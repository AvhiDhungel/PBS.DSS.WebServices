using Microsoft.AspNetCore.Mvc;
using PBS.ConnectHub.Library;
using PBS.DSS.WebServices.Server.Extensions;

namespace PBS.DSS.WebServices.Server.Utilities
{
    public class ConnectReceiveMessage<T>(T o, string serial, string messageType)
    {
        private static readonly int TimeOut = 2 * 60 * 1000;
        private Activity Log { get; set; } = new Activity(serial, messageType);

        public string SerialNumber { get; set; } = serial;
        public string MessageType { get; set; } = messageType;

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

        #region Logging
        public void LogMessage(string msg) => Log.LogMessage(msg);
        public void LogNewLine() => Log.LogNewLine();
        public void LogSerialized(object o) => Log.LogMessage(System.Text.Json.JsonSerializer.Serialize(o));
        public void LogSerializedWithMessage(object o, string message)
        {
            LogMessage(message);
            LogSerialized(o);
            LogNewLine();
        }
        public void LogException(Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
            Log.LogException(ex);
        }
        public void LogMessageHeader(MessageHeaderV2 msgHeader)
        {
            LogMessage($"Received {msgHeader.ParsedMessageType()} from ConnectHub");
            LogNewLine();
        }
        public void UpdateLog()
        {
            if (HasError) LogMessage($"Connect Hub Call finished with an error: {ErrorMessage}");
            Log.Update();
        }
        #endregion
    }
}
