namespace PBS.DSS.Shared.Models.WorkItems
{
    public class Appointment
    {
        public Guid Id { get; set; } = Guid.Empty;
        public Guid ContactRef { get; set; } = Guid.Empty;
        public Guid VehicleRef { get; set; } = Guid.Empty;
        public DateTimeOffset AppointmentTime { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset PickupDate { get; set; } = DateTimeOffset.UtcNow;

        public bool IsCheckedIn { get; set; } = false;
        public bool IsCanceled { get; set; } = false;

        public bool SelfCheckInEnabled { get; set; } = false;
        public bool CanReschedule { get; set; } = false;
        public bool CanCancel { get; set; } = false;

        public int Odometer { get; set; } = 0;
        public string AppointmentNumber { get; set; } = string.Empty;
        public string DropOffInstructions { get; set; } = string.Empty;
        public string ShopBanner { get; set; } = string.Empty;

        public Contact ContactInfo { get; set; } = new Contact();
        public Vehicle Vehicle { get; set; } = new Vehicle();

        public List<RequestLine> Requests { get; set; } = [];

        public bool IsValid() => Id != Guid.Empty;

        public static Appointment GenerateDummy()
        {
            var appt = new Appointment();

            appt.AppointmentNumber = "81881";
            appt.SelfCheckInEnabled = true;
            appt.CanReschedule = true;
            appt.CanCancel = true;
            appt.DropOffInstructions = "Please park your vehicle on the left of the drive through and drop the keys in the box after you've checked in";

            var req1 = new RequestLine();
            req1.Description = "Lube Oil & Filter - 15 Point Inspection - Reset Maintenance Reminder if required";
            req1.AWRStatus = AWRStatuses.Approved;

            var req2 = new RequestLine();
            req2.Description = "Checking the A/C system for leaks and performance. Includes refrigerant recharge if needed.";
            req2.AWRStatus = AWRStatuses.Approved;

            var req3 = new RequestLine();
            req3.Description = "Flushing old brake fluid and replacing it to keep your brakes functioning properly. Includes a basic inspection of pads and rotors.";
            req3.AWRStatus = AWRStatuses.Approved;

            var req4 = new RequestLine();
            req4.Description = "Draining old coolant and refilling with new fluid to prevent overheating. Basic system inspection included.";
            req4.AWRStatus = AWRStatuses.Approved;

            appt.Requests.Add(req1);
            appt.Requests.Add(req2);
            appt.Requests.Add(req3);
            appt.Requests.Add(req4);

            appt.ContactInfo = Contact.GenerateDummy();
            appt.Vehicle = Vehicle.GenerateDummy();


            return appt;
        }
    }
}