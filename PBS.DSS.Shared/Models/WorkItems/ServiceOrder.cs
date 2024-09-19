namespace PBS.DSS.Shared.Models.WorkItems
{
    public class ServiceOrder
    {
        public Guid Id { get; set; } = Guid.Empty;
        public Guid ContactRef { get; set; } = Guid.Empty;
        public Guid VehicleRef { get; set; } = Guid.Empty;
        public string SONumber { get; set; } = string.Empty;
        public string AdditionalComments { get; set; } = string.Empty;
        public decimal SubTotal { get; set; } = decimal.Zero;
        public decimal TaxTotal { get; set; } = decimal.Zero;
        public decimal FeesTotal { get; set; } = decimal.Zero;
        public decimal GrandTotal { get; set; } = decimal.Zero;

        public Contact ContactInfo { get; set; } = new Contact();
        public Vehicle Vehicle { get; set; } = new Vehicle();

        public List<RequestLine> Requests { get; set; } = [];
        public List<Attachment> Attachments { get; set; } = [];
    }
}
