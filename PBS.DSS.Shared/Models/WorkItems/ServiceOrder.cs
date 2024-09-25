using PBS.DSS.Shared.Enums;

namespace PBS.DSS.Shared.Models.WorkItems
{
    public class ServiceOrder
    {
        public Guid Id { get; set; } = Guid.Empty;
        public Guid ContactRef { get; set; } = Guid.Empty;
        public Guid VehicleRef { get; set; } = Guid.Empty;

        public bool IsOpen { get; set; } = false;
        public string SONumber { get; set; } = string.Empty;
        public string AdvisorName { get; set; } = string.Empty;
        public string AdditionalComments { get; set; } = string.Empty;
        public string ShopBanner { get; set; } = string.Empty;

        public double SubTotal { get; set; } = 0;
        public double TaxTotal { get; set; } = 0;
        public double FeesTotal { get; set; } = 0;
        public double GrandTotal { get; set; } = 0;

        public ServiceOrderTimeline Timeline { get; set; } = ServiceOrderTimeline.InspectionComplete;
        public Contact ContactInfo { get; set; } = new Contact();
        public Vehicle Vehicle { get; set; } = new Vehicle();

        public List<RequestLine> Requests { get; set; } = [];
        public List<Attachment> Attachments { get; set; } = [];

        public IEnumerable<RequestLine> ApprovedRequests { get => Requests.Where((x) => x.AWRStatus == AWRStatuses.Approved); }
        public IEnumerable<RequestLine> PendingRequests { get => Requests.Where((x) => x.AWRStatus == AWRStatuses.Pending); }
        public IEnumerable<RequestLine> RequestsMarkedForApproval { get => Requests.Where((x) => x.AWRStatus == AWRStatuses.Pending &&
                                                                           x.MarkedForApproval.HasValue && x.MarkedForApproval.Value); }

        public bool IsValid() => Id != Guid.Empty;

        public static ServiceOrder GenerateDummy()
        {
            var so = new ServiceOrder();

            so.Id = Guid.NewGuid();
            so.ContactRef = Guid.NewGuid();
            so.VehicleRef = Guid.NewGuid();
            so.SONumber = "81881";
            so.AdvisorName = "Brandon";
            so.AdditionalComments = "Some Comment that Brandon typed up but I don't want to type up";
            so.SubTotal = 300.20;
            so.TaxTotal = 29.5;
            so.FeesTotal = 15.87;
            so.GrandTotal = 345.57;
            so.IsOpen = true;

            var approved1 = new RequestLine();
            approved1.Description = "Lube Oil & Filter - 15 Point Inspection - Reset Maintenance Reminder if required";
            approved1.AWRStatus = AWRStatuses.Approved;

            var approved2 = new RequestLine();
            approved2.Description = "Checking the A/C system for leaks and performance. Includes refrigerant recharge if needed.";
            approved2.AWRStatus = AWRStatuses.Approved;

            var approved3 = new RequestLine();
            approved3.Description = "Flushing old brake fluid and replacing it to keep your brakes functioning properly. Includes a basic inspection of pads and rotors.";
            approved3.AWRStatus = AWRStatuses.Approved;

            var approved4 = new RequestLine();
            approved4.Description = "Draining old coolant and refilling with new fluid to prevent overheating. Basic system inspection included.";
            approved4.AWRStatus = AWRStatuses.Approved;

            var approved5 = new RequestLine();
            approved5.Description = "Perform engine oil and filter replacement using manufacturer-approved synthetic oil and OEM filter. Inspect and rotate all tires to ensure even wear, check tread depth, tire condition, and adjust tire pressure as necessary. Conduct a thorough inspection of the brake system, including pads, rotors, and brake fluid level, and top off fluids if needed. Inspect suspension components, steering linkages, and ensure proper alignment. Check all belts and hoses for signs of wear or deterioration. Perform cabin air filter and engine air filter replacement. Inspect the exhaust system for leaks or damage. Confirm proper operation of lights, wipers, and other vehicle safety features. Reset the maintenance minder system and perform a road test to verify proper operation of the vehicle post-service. Document any additional issues identified during the inspection for customer review and approval.";
            approved5.AWRStatus = AWRStatuses.Approved;

            var pending1 = new RequestLine();
            pending1.Description = "Urgent replacement of worn brake pads. Necessary for safe driving. Includes inspection of rotors and brake fluid.";
            pending1.EstimatedLabour = 50;
            pending1.EstimatedParts = 100;
            pending1.AWRStatus = AWRStatuses.Pending;
            pending1.Priority = RecommendedPriority.High;

            var pending2 = new RequestLine();
            pending2.Description = " Replace the cabin air filter to remove accumulated dust and debris that affect air quality inside the vehicle. ";
            pending2.EstimatedLabour = 50;
            pending2.EstimatedParts = 100;
            pending2.AWRStatus = AWRStatuses.Pending;
            pending2.Priority = RecommendedPriority.Medium;

            var pending3 = new RequestLine();
            pending3.Description = "Draining old transmission fluid and replacing it with new fluid. Helps improve shifting performance.";
            pending3.EstimatedLabour = 50;
            pending3.EstimatedParts = 100;
            pending3.AWRStatus = AWRStatuses.Pending;
            pending3.Priority = RecommendedPriority.Low;

            so.Requests.Add(approved1);
            so.Requests.Add(approved2);
            so.Requests.Add(approved3);
            so.Requests.Add(approved4);
            so.Requests.Add(approved5);
            so.Requests.Add(pending1);
            so.Requests.Add(pending2);
            so.Requests.Add(pending3);

            so.Vehicle = Vehicle.GenerateDummy();

            return so;
        }
    }
}
