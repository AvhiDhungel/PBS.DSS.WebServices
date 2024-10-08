using Microsoft.JSInterop;
using PBS.Blazor.Framework.Helpers;

namespace PBS.Blazor.ClientFramework.Services
{
    public sealed class InterOpService(IJSRuntime jsRuntime)
    {
        private readonly IJSRuntime _jsRuntime = jsRuntime;

        public async Task ToggleTheme(bool isDark)
        {
            await _jsRuntime.InvokeVoidAsync("toggleTheme", isDark);
        }

        public async ValueTask<bool> IsMobile()
        {
            return await _jsRuntime.InvokeAsync<bool>("isDevice");
        }

        public async Task CopyToClipboard(string str)
        {
            await _jsRuntime.InvokeVoidAsync("copyTextToClipboard", str);
        }

        public async Task OpenLinkInNewTab(string url)
        {
            await _jsRuntime.InvokeVoidAsync("openLinkInNewTab", url);
        }

        public async Task OpenPDFInNewTab(byte[] content)
        {
            await _jsRuntime.InvokeVoidAsync("openPDFInNewTab", content);
        }

        public async Task RequestElementFullScreen(string elementName)
        {
            await _jsRuntime.InvokeVoidAsync("requestFullScreen", elementName);
        }

        public async Task DownloadICS(ICSDownloadArgs args)
        {
            List<string> jsArgs =
            [
                args.StartDate.ToString("yyyyMMddTHHmmss"),
                args.EndDate.ToString("yyyyMMddTHHmmss"),
                DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ"),
                args.Summary,
                args.Description,
                args.Reference,
            ];

            await _jsRuntime.InvokeVoidAsync("generateICS", jsArgs);
        }
    }
}
