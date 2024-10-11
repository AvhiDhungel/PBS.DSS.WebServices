using System.Text;
using System.Text.Json;
using PBS.Blazor.Framework.Resources;
using PBS.Blazor.Framework.Helpers;
using PBS.Blazor.Framework.Interfaces;

namespace PBS.Blazor.ClientFramework.Services
{
    public sealed class SharedStateService<TSharedState>(TSharedState sharedState, SessionStorageService sessionStorageService) where TSharedState : ISharedState
    {
        private readonly SessionStorageService _sessionStorageService = sessionStorageService;

        public TSharedState SharedState { get; set; } = sharedState;
        public Action? OnRefreshMainLayout { get; set; }

        public void RefreshMainLayout() => OnRefreshMainLayout?.Invoke();

        #region StateModel
        public bool HasModel() => SharedState.Model != null;
        public void SetModel(object model) => SharedState.Model = model;

        public T? GetModel<T>()
        {
            if (SharedState.Model == null) return default;

            if (SharedState.Model.GetType() == typeof(T)) return (T)SharedState.Model;
            if (SharedState.Model.GetType() == typeof(JsonElement))
            {
                try { return ((JsonElement)SharedState.Model).Deserialize<T>(); }
                catch { return default; }
            }
            return default;
        }

        public async Task SaveToSession() => await _sessionStorageService.SaveSharedState(SharedState);
        public async Task GetFromSession() => SharedState = await _sessionStorageService.GetSharedState<TSharedState>() ?? SharedState;
        #endregion

        #region Errors
        private readonly StringBuilder _errors = new();

        public void AddError(string err) => _errors.AppendLine(err);
        public void ClearErrors() => _errors.Clear();

        public string GetErrors() => _errors.ToString();
        public bool HasErrors() => _errors.Length > 0;

        public void SetInvalidSerialError() => AddError(string.Format(FrameworkResources.SerialNumber0IsInvalid, SharedState.SerialNumber));

        public bool ValidateSerialNumber(string serial)
        {
            var isValid = ValidationHelper.ValidateSerialNumber(serial);
            if (!isValid) SetInvalidSerialError(); else SharedState.SerialNumber = serial;

            return isValid;
        }
        #endregion
    }
}