namespace PBS.DSS.Shared.Models.WorkItems
{
    public class RequestLine
    {
        public Guid RequestRef { get; set; } = Guid.Empty;
        public Guid OpCodeRef { get; set; } = Guid.Empty;
        public string OpCode { get; set; } = string.Empty;   
        public string Description { get; set; } = string.Empty;
        public double EstimatedLabour { get; set; } = 0;
        public double EstimatedParts { get; set; } = 0;
        public AWRStatuses AWRStatus { get; set; } = AWRStatuses.Approved;
        public RecommendedPriority Priority { get; set; } = RecommendedPriority.None;

        public bool? MarkedForApproval { get; set; }
    }

    public enum AWRStatuses
    {
        Approved,
        Pending,
        Deferred,
        Declined
    }

    public enum RecommendedPriority
    {
        None,
        Low,
        Medium,
        High
    }
}
