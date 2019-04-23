using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MvcApp.DependencyInjection;

namespace MvcApp.ComPort
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddComPort(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.TryAddSingleton<ComPortAccessor>();
            
            serviceCollection.AddValidatableConfigObject<SystemConfig>(configuration,"System");
            serviceCollection.AddValidatableConfigObject<ArduinoSerialConfig>(configuration, "ArduinoSerial");
            
            return serviceCollection;
        }
    }


}
