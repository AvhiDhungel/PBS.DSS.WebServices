using PBS.DigitalServiceSuite.Shared.Models.WorkItems;

namespace PBS.DigitalServiceSuite.Shared.Args
{
    public class DocumentSignatureArgs
    {
        public Guid WorkItemRef { get; set; } = Guid.Empty;
        public string WorkItemNumber { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public Signature Signature { get; set; } = new();
    }
}
