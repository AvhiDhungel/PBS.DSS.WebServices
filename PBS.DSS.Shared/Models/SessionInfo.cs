using PBS.DSS.Shared.Helpers;
using System.Text;

namespace PBS.DSS.Shared.Models
{
    public class SessionInfo
    {
        public Dictionary<ModelTypes, object> Models { get; set; } = [];

        public string SerialNumber { get; set; } = string.Empty;
        public Guid WorkItemRef { get; set; } = Guid.Empty;
        public string Banner { get; set; } = string.Empty;

        public bool IsValid { get => SerialNumber.HasValue(); }

        public enum ModelTypes
        {
            ServiceOrder,
            Appointment,
            InspectionDocument
        }

        public void AddModel(ModelTypes type, object model) => Models[type] = model;

        public T? GetModel<T>(ModelTypes type)
        {
            if (!Models.TryGetValue(type, out object? model)) return default;
            if (model.GetType() != typeof(T)) return default;

            return (T)model;
        }

        public string GetBase64Document(ModelTypes type)
        {
            return Models.TryGetValue(type, out object? model) ? (model?.ToString() ?? string.Empty) : string.Empty;
        }

        #region Errors
        private readonly StringBuilder _errors = new();

        public void AddError(string err) => _errors.AppendLine(err);
        public void ClearErrors() => _errors.Clear();

        public string GetErrors() => _errors.ToString();
        public bool HasErrors() => _errors.Length > 0;

        public void SetInvalidSerialError() => AddError($"Serial Number {SerialNumber} is invalid.");
        public void SetInvalidServiceOrderError() => AddError($"We were unable to locate your Service Order");
        public void SetInvalidAppointmentError() => AddError($"We were unable to locate your Appointment");
        #endregion

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
