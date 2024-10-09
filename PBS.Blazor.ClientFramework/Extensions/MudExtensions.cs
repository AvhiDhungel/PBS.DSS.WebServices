using MudBlazor;
using PBS.Blazor.Framework.Extensions;
using PBS.Blazor.ClientFramework.MudComponents;
using PBS.Blazor.Framework.Enums;

namespace PBS.Blazor.ClientFramework.Extensions
{
    public static partial class MudExtensions
    {
        #region Busy Dialog
        public static async Task<IDialogReference> ShowBusyDialog(this IDialogService s)
        {
            return await ShowBusyDialog(s, string.Empty);
        }

        public static async Task<IDialogReference> ShowBusyDialog(this IDialogService s, string message)
        {
            var p = new DialogParameters<MudBusyDialog>();
            if (message.HasValue()) p.Add(x => x.Message, message);

            return await s.ShowAsync<MudBusyDialog>("", p);
        }

        public static async Task PerformBusy(this IDialogService s, Task t)
        {
            await s.PerformBusy(t, string.Empty);
        }

        public static async Task PerformBusy(this IDialogService s, Task t, string message)
        {
            var d = await s.ShowBusyDialog(message);
            await t;
            d.Close();
        }

        public static async Task<T> PerformBusyResult<T>(this IDialogService s, Task<T> t)
        {
            return await s.PerformBusyResult<T>(t, string.Empty);
        }

        public static async Task<T> PerformBusyResult<T>(this IDialogService s, Task<T> t, string message)
        {
            var d = await s.ShowBusyDialog(message);
            var result = await t;

            d.Close();
            return result;
        }
        #endregion

        #region Mud Dialog
        public static T? GetValue<T>(this DialogResult? d)
        {
            if (d == null || d.Canceled || d.Data == null || d.Data.GetType() != typeof(T)) return default;
            return (T)d.Data;
        }
        #endregion

        #region Mud Class
        public static string GetClass(this MudClasses c)
        {
            return c switch
            {
                MudClasses.MudDialog => "mud-dialog",
                MudClasses.MudPaperSticky => "mud-paper-sticky",
                MudClasses.MudCard => "mud-card",
                MudClasses.MudCardDetail => "mud-card-detail",
                MudClasses.MudButton => "mud-button",
                MudClasses.ColoredButton => "colored-button",
                MudClasses.ToggleButton => "toggle-button",
                MudClasses.PDFViewerContainer => "pdf-viewer-container",
                MudClasses.PDFFrame => "pdf-frame",
                MudClasses.MediaCarousel => "media-carousel",
                MudClasses.PinnedFooter => "page-footer-pinned",
                MudClasses.PageBanner => "page-banner-image",
                _ => string.Empty,
            };
        }
        #endregion
    }
}
