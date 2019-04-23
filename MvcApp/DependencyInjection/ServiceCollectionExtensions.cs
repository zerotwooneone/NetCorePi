using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MvcApp.JsonConfig;

namespace MvcApp.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigObject<T>(this IServiceCollection serviceCollection, IConfiguration configuration, string sectionPath) where T : class
        {
            serviceCollection.Configure<T>(configuration.GetSection(sectionPath));
            
            serviceCollection.TryAddSingleton<T>(sp=>sp.GetRequiredService<IOptionsMonitor<T>>().CurrentValue);
            
            return serviceCollection;
        }

        public static IServiceCollection AddValidatableConfigObject<T>(this IServiceCollection serviceCollection, IConfiguration configuration, string sectionPath) where T : class, IValidatableConfig
        {
            serviceCollection.AddSingleton<IValidatableConfig>(sp=>sp.GetRequiredService<IOptionsMonitor<T>>().CurrentValue);
            
            return serviceCollection.AddConfigObject<T>(configuration, sectionPath);
        }
    }
}
