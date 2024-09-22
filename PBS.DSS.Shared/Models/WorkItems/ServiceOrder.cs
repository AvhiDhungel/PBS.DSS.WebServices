namespace PBS.DSS.Shared.Models.WorkItems
{
    public class ServiceOrder
    {
        public Guid Id { get; set; } = Guid.Empty;
        public Guid ContactRef { get; set; } = Guid.Empty;
        public Guid VehicleRef { get; set; } = Guid.Empty;

        public string SONumber { get; set; } = string.Empty;
        public string AdditionalComments { get; set; } = string.Empty;
        public string ShopBanner { get; set; } = string.Empty;

        public double SubTotal { get; set; } = 0;
        public double TaxTotal { get; set; } = 0;
        public double FeesTotal { get; set; } = 0;
        public double GrandTotal { get; set; } = 0;

        public Contact ContactInfo { get; set; } = new Contact();
        public Vehicle Vehicle { get; set; } = new Vehicle();

        public List<RequestLine> Requests { get; set; } = [];
        public List<Attachment> Attachments { get; set; } = [];

        public IEnumerable<RequestLine> ApprovedRequests { get => Requests.Where((x) => x.AWRStatus == AWRStatuses.Approved); }
        public IEnumerable<RequestLine> PendingRequests { get => Requests.Where((x) => x.AWRStatus == AWRStatuses.Pending); }

        public bool IsValid() => Id != Guid.Empty;

        public static ServiceOrder GenerateDummy()
        {
            var so = new ServiceOrder();

            so.Id = Guid.NewGuid();
            so.ContactRef = Guid.NewGuid();
            so.VehicleRef = Guid.NewGuid();
            so.SONumber = "81881";
            so.AdditionalComments = "Some Comment that Brandon typed up but I don't want to type up";
            so.SubTotal = 300.20;
            so.TaxTotal = 29.5;
            so.FeesTotal = 15.87;
            so.GrandTotal = 345.57;

            var approved = new RequestLine();
            approved.Description = "Lube Oil & Filter - 15 Point Inspection - Reset Maintenance Reminder if required";
            approved.AWRStatus = AWRStatuses.Approved;

            var pending1 = new RequestLine();
            pending1.Description = "Perform Cooling System Fluid Exchange/Flush";
            pending1.EstimatedLabour = 50;
            pending1.EstimatedParts = 100;
            pending1.AWRStatus = AWRStatuses.Pending;
            pending1.Priority = RecommendedPriority.High;

            var pending2 = new RequestLine();
            pending2.Description = "Air Conditioning Recovery, Evacuation and Recharge";
            pending2.EstimatedLabour = 50;
            pending2.EstimatedParts = 100;
            pending2.AWRStatus = AWRStatuses.Pending;
            pending2.Priority = RecommendedPriority.Medium;

            so.Requests.Add(approved);
            so.Requests.Add(pending1);
            so.Requests.Add(pending2);

            so.Vehicle = Vehicle.GenerateDummy();

            return so;
        }
    }
}
