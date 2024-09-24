namespace PBS.DSS.WebServices.Server.Utilities
{
    public class Utility
    {
        public static string DBConnectionString()
        {
            {
                return Environment.GetEnvironmentVariable("DBConnectionString") ?? string.Empty;
            }
        }

        public static string WebAppointmentsDBConnectionString()
        {
            {
                return Environment.GetEnvironmentVariable("WebAppointmentsDBConnectionString") ?? string.Empty;
            }
        }

        public static string InvoiceHubDBConnectionString()
        {
            {
                return Environment.GetEnvironmentVariable("InvoiceHubDBConnectionString") ?? string.Empty;
            }
        }
    }
}