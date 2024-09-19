namespace PBS.DSS.Shared.Models.WorkItems
{
    public class Attachment
    {
        public Guid AttachmentRef { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public byte[] Content { get; set; } = [];
        public FileTypes FileType { get; set; } = FileTypes.Document;
        public DocumentTypes DocumentType { get; set; } = DocumentTypes.None;
    }

    public enum FileTypes
    {
        Document,
        Image,
        Video,
        Other
    }

    public enum DocumentTypes
    {
        None,
        CustomerCopy,
        EstimateCopy,
        Inspection
    }
}
