using MudBlazor;
using PBS.DSS.Shared;
using PBS.DSS.WebServices.Client.Components;
using System;

namespace PBS.DSS.WebServices.Client
{
    public static partial class Extensions
    {
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

        public static T? GetValue<T>(this DialogResult? d)
        {
            if (d == null || d.Canceled || d.Data == null || d.Data.GetType() != typeof(T)) return default;
            return (T)d.Data;
        }
    }
}
