namespace PBS.DSS.Shared.Models.WorkItems
{
    public class Appointment
    {
        public Guid Id { get; set; } = Guid.Empty;
        public Guid ContactRef { get; set; } = Guid.Empty;
        public Guid VehicleRef { get; set; } = Guid.Empty;
        public string AppointmentNumber { get; set; } = string.Empty;
        public string AdditionalComments {  get; set; } = string.Empty;
        public DateTime AppointmentTimeUTC { get; set; } = DateTime.UtcNow;

        public Contact ContactInfo { get; set; } = new Contact();
        public Vehicle Vehicle { get; set; } = new Vehicle();

        public List<RequestLine> Requests { get; set; } = [];
    }
}