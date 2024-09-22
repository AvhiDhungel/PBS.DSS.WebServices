using PBS.DSS.Shared.Enums;

namespace PBS.DSS.Shared.Models.States
{
    public class SharedState
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string Banner { get; set; } = string.Empty;
        
        public Dictionary<SharedModelTypes, object> Models { get; set; } = [];
    }
}
