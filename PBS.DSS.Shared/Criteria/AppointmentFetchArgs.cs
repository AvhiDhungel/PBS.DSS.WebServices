namespace PBS.DSS.Shared.Criteria
{
    public class AppointmentFetchArgs
    {
        public Guid AppointmentRef { get; set; } = Guid.Empty;
        public string SerialNumber { get; set; } = string.Empty;
    }
}
