using PBS.DSS.Shared.Helpers;
using System.Text;

namespace PBS.DSS.Shared.Models
{
    public class SessionInfo
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string Banner { get; set; } = string.Empty;

        private readonly StringBuilder _errors = new();

        public void AddError(string err) => _errors.AppendLine(err);
        public void ClearErrors() => _errors.Clear();

        public string GetErrors() => _errors.ToString();
        public bool HasErrors() => _errors.Length > 0;

        public void SetInvalidSerialError() => AddError($"Serial Number {SerialNumber} is invalid.");
        public void SetInvalidServiceOrderError() => AddError($"We were unable to locate your Service Order");
        public void SetInvalidAppointmentError() => AddError($"We were unable to locate your Appointment");

        #region Validation
        public bool ValidateSerialNumber(string serial)
        {
            var isValid = ValidationHelper.ValidateSerialNumber(serial);
            if (!isValid) SetInvalidSerialError(); else SerialNumber = serial;

            return isValid;
        }

        public bool ValidateServiceOrderRef(string workItemRef, out Guid parsedWorkItemRef)
        {
            var isValid = ValidationHelper.ValidateWorkItemRef(workItemRef, out parsedWorkItemRef);
            if (!isValid) SetInvalidServiceOrderError();

            return isValid;
        }

        public bool ValidateAppointmentRef(string workItemRef, out Guid parsedWorkItemRef)
        {
            var isValid = ValidationHelper.ValidateWorkItemRef(workItemRef, out parsedWorkItemRef);
            if (!isValid) SetInvalidAppointmentError();

            return isValid;
        }
        #endregion
    }
}
