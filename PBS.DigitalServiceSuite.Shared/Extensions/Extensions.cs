using PBS.Blazor.Framework.Extensions;
using PBS.DigitalServiceSuite.Shared.Models.WorkItems;

namespace PBS.DigitalServiceSuite.Shared
{
    public static partial class Extensions
    {
        #region Enum
        public static string ToLocalizedString(this Enum e)
        {
            var key = e.ToString() ?? string.Empty;
            if (!key.HasValue()) return string.Empty;

            return Resources.Resources.ResourceManager.GetString(key) ?? key.ToString();
        }
        #endregion

        #region Appointment Statuses
        public static bool IsOpen(this Appointment a) => a.Status == Enums.AppointmentStatuses.Open;
        public static bool IsArrived(this Appointment a) => a.Status == Enums.AppointmentStatuses.Arrived;
        public static bool IsDeleted(this Appointment a) => a.Status == Enums.AppointmentStatuses.Deleted;
        public static bool IsPending(this Appointment a) => a.Status == Enums.AppointmentStatuses.Pending;
        public static bool IsConfirmed(this Appointment a) => a.Status == Enums.AppointmentStatuses.Confirmed;
        public static bool IsCheckedIn(this Appointment a) => a.Status == Enums.AppointmentStatuses.CheckedIn;
        public static bool CanCheckIn(this Appointment a)
        {
            var dayDiff = DateTime.UtcNow.Subtract(a.AppointmentTime.UtcDateTime).TotalDays;
            if (dayDiff < -1 || dayDiff > 1) return false;

            return a.SelfCheckInEnabled && a.IsOpen();
        }
        public static bool CanCancel(this Appointment a)
        {
            if (a.AppointmentTime.UtcDateTime < DateTime.UtcNow.AddDays(-1)) return false;
            return a.IsOpen() || a.IsConfirmed();
        }
        public static bool CanReschedule(this Appointment a)
        {
            if (a.AppointmentTime.UtcDateTime < DateTime.UtcNow.AddDays(-1)) return false;
            return a.IsOpen() || a.IsConfirmed();
        }
        #endregion

        #region Requests
        public static bool IsApproved(this RequestLine req)
        {
            return req.AWRStatus == AWRStatuses.Approved;
        }
        #endregion

        #region CSS
        public static string FriendlyText(this RecommendedPriority p)
        {
            switch (p)
            {
                case RecommendedPriority.Medium:
                    return Resources.Resources.AttentionRequired;
                case RecommendedPriority.High:
                    return Resources.Resources.ImmediateAttentionRequired;
                case RecommendedPriority.Low:
                    return "Recommended";
                default:
                    return string.Empty;
            }
        }

        public static string LabelStyle(this RecommendedPriority p)
        {
            var baseStyle = "width:fit-content; border-radius:8px;";
            baseStyle += "border-top-width:1px; border-bottom-width:1px; border-left-width:1px; border-right-width:1px;";

            switch (p)
            {
                case RecommendedPriority.Medium:
                    return $"{baseStyle} border-color:darkorange; color:darkorange;";
                case RecommendedPriority.High:
                    return $"{baseStyle} border-color:darkred; color:indianred;";
                case RecommendedPriority.Low:
                    return $"{baseStyle} border-color:black;";
                default:
                    return string.Empty;
            }
        }

        public static string GetColor(this RecommendedPriority p)
        {
            switch (p)
            {
                case RecommendedPriority.Medium:
                    return "orange;";
                case RecommendedPriority.High:
                    return "red;";
                default:
                    return "inherit;";
            }
        }
        #endregion
    }
}
