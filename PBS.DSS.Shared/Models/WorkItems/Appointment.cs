namespace PBS.DSS.Shared.Models.WorkItems
{
    public class Appointment
    {
        public Guid Id { get; set; } = Guid.Empty;
        public Guid ContactRef { get; set; } = Guid.Empty;
        public Guid VehicleRef { get; set; } = Guid.Empty;
        public DateTimeOffset AppointmentTime { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset PickupDate { get; set; } = DateTimeOffset.UtcNow;

        public bool CheckInAvailable { get; set; } = false;
        public bool CanReschedule { get; set; } = false;
        public bool CanCancel { get; set; } = false;

        public int Odometer { get; set; } = 0;
        public string AppointmentNumber { get; set; } = string.Empty;
        public string AdditionalComments { get; set; } = string.Empty;
        public string ShopBanner { get; set; } = string.Empty;

        public Contact ContactInfo { get; set; } = new Contact();
        public Vehicle Vehicle { get; set; } = new Vehicle();

        public List<RequestLine> Requests { get; set; } = [];

        public bool IsValid() => Id != Guid.Empty;

        public static Appointment GenerateDummy()
        {
            var appt = new Appointment();

            appt.AppointmentNumber = "81881";
            appt.CheckInAvailable = true;
            appt.CanReschedule = true;
            appt.CanCancel = true;
            appt.AdditionalComments = "Please park your vehicle on the left of the drive through and drop the keys in the box after you've checked in";

            var req = new RequestLine();
            req.Description = "Lube Oil & Filter - 15 Point Inspection - Reset Maintenance Reminder if required";
            req.AWRStatus = AWRStatuses.Approved;

            appt.Requests.Add(req);

            appt.ContactInfo = Contact.GenerateDummy();
            appt.Vehicle = Vehicle.GenerateDummy();


            return appt;
        }
    }
}