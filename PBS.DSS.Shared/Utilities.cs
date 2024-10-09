using PBS.Blazor.Framework.Extensions;

namespace PBS.DSS.Shared
{
    public class Utilities
    {
        private static string C_WebAppointmentsURL = "https://webappointments.pbssystems.com";

        public static string GetWebAppointmentsURL() => C_WebAppointmentsURL;

        public static string DecodeRequestor(string requestor)
        {
            if (requestor.IsNullOrWhitespace()) return string.Empty;

            var bytes = Convert.FromBase64String(requestor);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
