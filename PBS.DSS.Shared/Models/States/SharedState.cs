using PBS.DSS.Shared.Enums;
using PBS.DSS.Shared.Models.WorkItems;

namespace PBS.DSS.Shared.Models.States
{
    public class SharedState
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string Banner { get; set; } = string.Empty;
     
        public Guid WorkItemRef { get; set; } = Guid.Empty;
        public WorkItemTypes WorkItemType { get; set; } = WorkItemTypes.None;
        public object? Model { get; set; }

        public Dictionary<DocumentTypes, Signature> Signatures { get; set; } = [];
    }
}
