namespace PBS.DSS.Shared.Criteria
{
    public class ServiceOrderFetchArgs
    {
        public Guid ServiceOrderRef { get; set; } = Guid.Empty;
        public string SerialNumber { get; set; } = string.Empty;
    }
}
