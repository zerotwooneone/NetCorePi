using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MvcApp.DependencyInjection;

namespace MvcApp.ComPort
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddComPort(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.TryAddSingleton<ComPortAccessor>();
            serviceCollection.TryAddSingleton<IHostedService,ComPortService>();
            
            serviceCollection.AddValidatableConfigObject<SystemConfig>(configuration,"System");
            serviceCollection.AddValidatableConfigObject<ArduinoSerialConfig>(configuration, "ArduinoSerial");
            
            return serviceCollection;
        }
    }


}
