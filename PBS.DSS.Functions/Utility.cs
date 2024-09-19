namespace PBS.DSS.Functions
{
    class Utility
    {
        public static string GetConnectionString()
        {
            return System.Environment.GetEnvironmentVariable("DBConnectionString");
        }
    }
}
