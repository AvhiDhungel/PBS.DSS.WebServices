namespace PBS.DSS.Shared.Helpers
{
    public class ValidationHelper
    {
        public static bool ValidateSerialNumber(string serial)
        {
            if (serial.IsNullOrWhitespace()) return false;
            if (!serial.Length.IsInSet(4, 7)) return false;
            if (serial.Length == 4 && !serial.IsInteger()) return false;

            if (serial.Length == 7)
            {
                if (!serial.EndsWith(".QA", StringComparison.OrdinalIgnoreCase)) return false;
                if (!serial.Left(4).IsInteger()) return false;
            }

            return true;
        }

        public static bool ValidateWorkItemRef(string workItemRef, out Guid parsedWorkItemRef)
        {
            if (!workItemRef.IsNullOrWhitespace() && Guid.TryParse(workItemRef, out parsedWorkItemRef)) return true;

            parsedWorkItemRef = Guid.Empty;
            return false;
        }
    }
}
