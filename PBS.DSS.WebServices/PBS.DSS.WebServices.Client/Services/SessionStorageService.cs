using Blazored.SessionStorage;
using System.Text.Json;
using System.Text;
using System.Globalization;
using PBS.DSS.Shared;
using PBS.DSS.Shared.Models.States;

namespace PBS.DSS.WebServices.Client.Services
{
    public sealed class SessionStorageService(ISessionStorageService storageService)
    {
        private const string C_SessionCultureKey = "PBS_DSS_CultureString";
        private const string C_LightThemeKey = "PBS_DSS_LightTheme";
        private const string C_SharedStateKey = "PBS_DSS_SharedState";

        private readonly ISessionStorageService _sessionStorageService = storageService;

        public async Task SaveToSessionAsync<T>(string key, T item)
        {
            var json = JsonSerializer.Serialize(item);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var base64 = Convert.ToBase64String(jsonBytes);

            await _sessionStorageService.SetItemAsync(key, base64);
        }

        public async Task<T?> ReadFromSessionAsync<T>(string key)
        {
            var base64 = await _sessionStorageService.GetItemAsync<string>(key);
            if (base64 == null) { return default; }

            var jsonBytes = Convert.FromBase64String(base64);
            var json = Encoding.UTF8.GetString(jsonBytes);
            var item = JsonSerializer.Deserialize<T>(json);

            return item;
        }

        public async Task SetSessionCulture(string cultureString) => await SaveToSessionAsync(C_SessionCultureKey, cultureString);
        public async Task<CultureInfo> GetSessionCulture()
        {
            var culture = new CultureInfo("en-CA");
            var cultureString = await ReadFromSessionAsync<string>(C_SessionCultureKey) ?? string.Empty;

            if (cultureString.HasValue()) culture = new CultureInfo(cultureString);

            return culture;
        }

        public async Task SetLightMode(bool isLightMode) => await SaveToSessionAsync(C_LightThemeKey, isLightMode);
        public async Task SetDarkMode(bool isDarkMode) => await SaveToSessionAsync(C_LightThemeKey, !isDarkMode);
        public async Task<bool> IsLightMode() => await ReadFromSessionAsync<bool>(C_LightThemeKey);
        public async Task<bool> IsDarkMode() => !(await IsLightMode());

        public async Task SaveSharedState(SharedState s) => await SaveToSessionAsync(C_SharedStateKey, s);
        public async Task<SharedState> GetSharedState() => await ReadFromSessionAsync<SharedState>(C_SharedStateKey) ?? new();
    }
}
