namespace PBS.Blazor.Framework.Interfaces
{
    public interface ISharedState
    {
        public string SerialNumber { get; set; }
        public object? Model { get; set; }
    }
}
