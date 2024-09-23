using MudBlazor;

namespace PBS.DSS.WebServices.Client
{
    public static partial class Extensions
    {
        public static T? GetValue<T>(this DialogResult? d)
        {
            if (d == null || d.Canceled || d.Data == null || d.DataType != typeof(T)) return default;
            return (T)d.Data;
        }
    }
}
