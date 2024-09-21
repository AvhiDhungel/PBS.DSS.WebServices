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

        public string Description { get => $"{Year} {Make} {Model}"; }

        public static Vehicle GenerateDummy()
        {
            return new Vehicle()
            {
                Id = Guid.NewGuid(),
                Year = "2025",
                Make = "Honda",
                Model = "CRV",
                Trim = "EX-L",
                VIN = "JH4KA4576KC031014"
            };
        }
    }
}