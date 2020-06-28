using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace StakeTradingBot
{
    public static class IServiceCollectionExtension
    {
        public static void RegisterAsImplementedInterfaces(this IServiceCollection services, Type type, ServiceLifetime lifetime)
        {
            var interfaces = type.GetTypeInfo().ImplementedInterfaces
                .Where(i => i != typeof(IDisposable) && (i.IsPublic));

            foreach (Type interfaceType in interfaces)
                services.Add(new ServiceDescriptor(interfaceType, type, lifetime));
        }
    }
}