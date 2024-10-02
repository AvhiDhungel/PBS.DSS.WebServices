using PBS.DSS.Shared.Models.WorkItems;

namespace PBS.DSS.Shared.Criteria
{
    public class ServiceOrderFetchArgs
    {
        public Guid ServiceOrderRef { get; set; } = Guid.Empty;
        public string SerialNumber { get; set; } = string.Empty;
    }

    public class ServiceOrderDocumentFetchArgs
    {
        public Guid ServiceOrderRef { get; set; } = Guid.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public DocumentTypes DocumentType { get; set; } = DocumentTypes.None;
    }
}
