using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace MvcApp.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigObject<T>(this IServiceCollection serviceCollection, IConfiguration configuration, string sectionPath) where T : class
        {
            serviceCollection.Configure<T>(configuration.GetSection(sectionPath));
            
            serviceCollection.TryAddSingleton<T>(sp=>ServiceProviderServiceExtensions.GetRequiredService<IOptionsMonitor<T>>(sp).CurrentValue);
            
            return serviceCollection;
        }
    }
}
