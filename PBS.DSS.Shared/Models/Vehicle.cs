namespace PBS.DSS.Shared.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Year { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Trim { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;
    }
}