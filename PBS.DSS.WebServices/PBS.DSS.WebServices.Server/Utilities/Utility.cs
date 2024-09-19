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
    }
}