using Microsoft.JSInterop;

namespace PBS.DSS.Shared.Services
{
    public sealed class InterOpService(IJSRuntime jsRuntime)
    {
        private readonly IJSRuntime _jsRuntime = jsRuntime;

        public async ValueTask<bool> GetIsMobile()
        {
            return await _jsRuntime.InvokeAsync<bool>("isDevice");
        }

        public async Task CopyToClipboard(string str)
        {
            await _jsRuntime.InvokeVoidAsync("copyTextToClipboard", str);
        }
    }
}
