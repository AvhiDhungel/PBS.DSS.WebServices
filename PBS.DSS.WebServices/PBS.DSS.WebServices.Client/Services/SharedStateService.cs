using PBS.DSS.Shared.Helpers;
using PBS.DSS.Shared.Enums;
using PBS.DSS.Shared.Models.States;
using PBS.DSS.Shared.Models.WorkItems;
using PBS.DSS.Shared;
using System.Text;
using PBS.DSS.Shared.Resources;

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

        public bool HasModel() => SharedState.Model != null;
        public async Task SaveModelToSession(ServiceOrder model) { SetModel(model); await SaveToSession(); }
        public async Task SaveModelToSession(Appointment model) { SetModel(model); await SaveToSession(); }

        public void SetModel(ServiceOrder model)
        {
            SharedState.Model = model;
            SharedState.WorkItemRef = model.Id;
            SharedState.WorkItemType = WorkItemTypes.ServiceOrder;
        }

        public void SetModel(Appointment model)
        {
            SharedState.Model = model;
            SharedState.WorkItemRef = model.Id;
            SharedState.WorkItemType = WorkItemTypes.Appointment;
        }

        public T? GetModel<T>()
        {
            if (SharedState.WorkItemType == WorkItemTypes.None) return default;
            if (SharedState.Model == null) return default;
            if (SharedState.Model.GetType() != typeof(T)) return default;

            return (T)SharedState.Model;
        }

        public async Task SaveToSession() => await _sessionStorageService.SaveSharedState(SharedState);
        public async Task GetFromSession() => SharedState = await _sessionStorageService.GetSharedState();

        #region Errors
        private readonly StringBuilder _errors = new();

        public void AddError(string err) => _errors.AppendLine(err);
        public void ClearErrors() => _errors.Clear();

        public string GetErrors() => _errors.ToString();
        public bool HasErrors() => _errors.Length > 0;

        public void SetInvalidSerialError() => AddError(string.Format(Resources.SerialNumber0IsInvalid, SharedState.SerialNumber));
        public void SetInvalidServiceOrderError() => AddError(Resources.UnableToLocationServiceOrder);
        public void SetInvalidAppointmentError() => AddError(Resources.UnableToLocationAppointment);
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
