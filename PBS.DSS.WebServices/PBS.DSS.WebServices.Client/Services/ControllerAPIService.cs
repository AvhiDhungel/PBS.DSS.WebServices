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

        public async Task<APIResponse<T>> Post<T, U>(U args, string controllerName, string methodName)
        {
            var resp = await _httpClient.PostAsJsonAsync($"/api/{controllerName}/{methodName}", args);
            var result = new APIResponse<T>();

            if (resp == null)
            {
                result.HasError = true;
                result.ErrorMessage = "Was not able to completed request";
            }
            else if (resp.IsSuccessStatusCode)
            {
                result.ResponseObject = await resp.Content.ReadFromJsonAsync<T>();
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = await resp.Content.ReadAsStringAsync();
            }

            return result;
        }

        #region Service Order
        public async Task<APIResponse<ServiceOrder>> FetchServiceOrder(ServiceOrderFetchArgs args)
        {
            return await Post<ServiceOrder, ServiceOrderFetchArgs>(args, "ServiceOrder", "FetchServiceOrder") ?? new();
        }
        #endregion

        #region Appointment
        public async Task<APIResponse<Appointment>> FetchAppointment(AppointmentFetchArgs args)
        {
            return await Post<Appointment, AppointmentFetchArgs>(args, "Appointment", "FetchAppointment") ?? new();
        }
        #endregion

        public class APIResponse<T>
        {
            public T? ResponseObject { get; set; }
            public bool HasError { get; set; } = false;
            public string ErrorMessage { get; set; } = string.Empty;

            public APIResponse() { }
            public APIResponse(T? o) { ResponseObject = o; }
        }
    }
}
