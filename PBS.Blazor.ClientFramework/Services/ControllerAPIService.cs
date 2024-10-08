using System.Net.Http.Json;

namespace PBS.Blazor.ClientFramework.Services
{
    public sealed class ControllerAPIService(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<T?> Get<T>(string controllerName, string methodName)
        {
            return await _httpClient.GetFromJsonAsync<T>($"/api/{controllerName}/{methodName}");
        }

        public async Task<T?> Get<T>(string controllerName, string methodName, List<KeyValuePair<string, string>> parameters)
        {
            var uri = new System.Text.StringBuilder($"/api/{controllerName}/{methodName}");

            foreach (var p in parameters)
            {
                uri.Append(parameters.IndexOf(p) == 1 ? '?' : '&');
                uri.Append($"{p.Key}={p.Value}");
            }

            return await _httpClient.GetFromJsonAsync<T>(uri.ToString());
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
                result.ErrorMessage = "Was not able to complete request";
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
