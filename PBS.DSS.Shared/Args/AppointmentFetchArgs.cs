using PBS.DSS.Shared.Models.WorkItems;

namespace PBS.DSS.Shared.Criteria
{
    public class AppointmentFetchArgs
    {
        public Guid AppointmentRef { get; set; } = Guid.Empty;
        public string SerialNumber { get; set; } = string.Empty;
    }

    public class AppointmentDocumentFetchArgs
    {
        public Guid AppointmentRef { get; set; } = Guid.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public DocumentTypes DocumentType { get; set; } = DocumentTypes.None;
    }
}
