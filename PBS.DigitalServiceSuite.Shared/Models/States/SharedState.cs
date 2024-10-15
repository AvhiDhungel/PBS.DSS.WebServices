using PBS.Blazor.Framework.Interfaces;
using PBS.DigitalServiceSuite.Shared.Enums;
using PBS.DigitalServiceSuite.Shared.Models.WorkItems;

namespace PBS.DigitalServiceSuite.Shared.Models.States
{
    public class SharedState : ISharedState
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string EncodedRequestor { get; set; } = string.Empty;
        public string Banner { get; set; } = string.Empty;
     
        public Guid WorkItemRef { get; set; } = Guid.Empty;
        public WorkItemTypes WorkItemType { get; set; } = WorkItemTypes.None;
        public object? Model { get; set; }

        public Dictionary<DocumentTypes, Signature> Signatures { get; set; } = [];
    }
}
