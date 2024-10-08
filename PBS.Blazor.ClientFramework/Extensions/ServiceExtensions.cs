using Microsoft.Extensions.DependencyInjection;
using PBS.Blazor.ClientFramework.Services;
using PBS.Blazor.Framework.Interfaces;

namespace PBS.Blazor.ClientFramework.Extensions
{
    public static partial class ServiceExtensions
    {
        public static void AddSharedState<TSharedState>(this IServiceCollection c) where TSharedState : class, ISharedState
        {
            c.AddScoped<TSharedState>();
            c.AddScoped<ISharedState, TSharedState>();
            c.AddScoped<SharedStateService<ISharedState>>();
            c.AddScoped<SharedStateService<TSharedState>>();
        }
    }
}
