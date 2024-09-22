using PBS.DSS.Shared.Helpers;
using PBS.DSS.Shared.Enums;
using PBS.DSS.Shared.Models.States;
using PBS.DSS.Shared;
using System.Text;

namespace PBS.DSS.WebServices.Client.Services
{
    public sealed class SharedStateService(SessionStorageService sessionStorageService)
    {
        private readonly SessionStorageService _sessionStorageService = sessionStorageService;

        public SharedState SharedState { get; set; } = new();
        public Action? RefreshMainLayout { get; set; }

        public bool HasBanner() => SharedState.Banner.HasValue();
        public string GetBanner() => SharedState.Banner;
        public void SetBanner(string banner)
        {
            SharedState.Banner = banner;
            RefreshMainLayout?.Invoke();
        }

        public void AddModel(SharedModelTypes type, object model) => SharedState.Models[type] = model;

        public T? GetModel<T>(SharedModelTypes type)
        {
            if (!SharedState.Models.TryGetValue(type, out object? model)) return default;
            if (model.GetType() != typeof(T)) return default;

            return (T)model;
        }

        public string GetBase64Document(SharedModelTypes type)
        {
            return SharedState.Models.TryGetValue(type, out object? model) ? (model?.ToString() ?? string.Empty) : string.Empty;
        }

        public async Task SaveToSession() => await _sessionStorageService.SaveSharedState(SharedState);
        public async Task GetFromSession() => SharedState = await _sessionStorageService.GetSharedState();

        #region Errors
        private readonly StringBuilder _errors = new();

        public void AddError(string err) => _errors.AppendLine(err);
        public void ClearErrors() => _errors.Clear();

        public string GetErrors() => _errors.ToString();
        public bool HasErrors() => _errors.Length > 0;

        public void SetInvalidSerialError() => AddError($"Serial Number {SharedState.SerialNumber} is invalid.");
        public void SetInvalidServiceOrderError() => AddError($"We were unable to locate your Service Order");
        public void SetInvalidAppointmentError() => AddError($"We were unable to locate your Appointment");
        #endregion

        #region Validation
        public bool ValidateSerialNumber(string serial)
        {
            var isValid = ValidationHelper.ValidateSerialNumber(serial);
            if (!isValid) SetInvalidSerialError(); else SharedState.SerialNumber = serial;

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
