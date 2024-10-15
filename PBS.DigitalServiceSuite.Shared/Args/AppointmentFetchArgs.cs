using PBS.DigitalServiceSuite.Shared.Models.WorkItems;

namespace PBS.DigitalServiceSuite.Shared.Args
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
