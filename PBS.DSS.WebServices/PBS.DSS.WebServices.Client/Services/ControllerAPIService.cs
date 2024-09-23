using PBS.DSS.Shared.Criteria;
using PBS.DSS.Shared.Models.WorkItems;
using System.Net.Http.Json;

namespace PBS.DSS.WebServices.Client.Services
{
    public sealed class ControllerAPIService(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<T?> Get<T>(string controllerName, string methodName)
        {
            return await _httpClient.GetFromJsonAsync<T>($"/api/{controllerName}/{methodName}");
        }

        public async Task<HttpResponseMessage?> Post<T>(T args, string controllerName, string methodName)
        {
            return await _httpClient.PostAsJsonAsync($"/api/{controllerName}/{methodName}", args);
        }

        public async Task<T?> Post<T, U>(U args, string controllerName, string methodName)
        {
            var resp = await _httpClient.PostAsJsonAsync($"/api/{controllerName}/{methodName}", args);
            if (resp == null || !resp.IsSuccessStatusCode) return default;

            return await resp.Content.ReadFromJsonAsync<T>();
        }

        public async Task<ServiceOrder> FetchServiceOrder(ServiceOrderFetchArgs args)
        {
            return await Post<ServiceOrder, ServiceOrderFetchArgs>(args, "ServiceOrder", "FetchServiceOrder") ?? new();
        }

        public async Task<Appointment> FetchAppointment(AppointmentFetchArgs args)
        {
            return await Post<Appointment, AppointmentFetchArgs>(args, "Appointment", "FetchAppointment") ?? new();
        }
    }
}
