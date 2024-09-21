namespace PBS.DSS.Shared.Models.WorkItems
{
    public class RequestLine
    {
        public Guid RequestRef { get; set; } = Guid.Empty;
        public Guid OpCodeRef { get; set; } = Guid.Empty;
        public string OpCode { get; set; } = string.Empty;   
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = decimal.Zero;
        public decimal EstimatedLabour { get; set; } = decimal.Zero;
        public decimal EstimatedParts { get; set; } = decimal.Zero;
        public AWRStatuses AWRStatus { get; set; } = AWRStatuses.Approved;
        public RecommendedPriority Priority { get; set; } = RecommendedPriority.None;

        public bool MarkedForApproval { get; set; } = false;
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
