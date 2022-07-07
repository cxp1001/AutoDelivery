using System.Reflection;
using AutoDelivery.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AutoDelivery.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RepositoryRegister(this IServiceCollection services)
        {
            var coreAssembly = Assembly.Load("AutoDelivery.Core");
            var implementationType = coreAssembly.GetTypes().FirstOrDefault(t => t.Name == "Repository`1");
            var interfaceType = implementationType.GetInterface("IRepository`1").GetGenericTypeDefinition();

            if (implementationType != null && interfaceType != null)
            {
                services.AddTransient(interfaceType, implementationType);
            }
            return services;
        }
        public static IServiceCollection ServiceRegister(this IServiceCollection services)
        {

             var serviceAssembly = Assembly.Load("AutoDelivery.Service");
            var implementationTypes = serviceAssembly.GetTypes().Where(t => t.IsAssignableTo(typeof(IocTag)) && !t.IsInterface && !t.IsAbstract);

            foreach(var implementationType in implementationTypes)
            {
                var interfaceType = implementationType.GetInterfaces().Where(i => i != typeof(IocTag)).FirstOrDefault();
                if (implementationType!=null&&interfaceType!=null)
                {
                    services.AddTransient(interfaceType, implementationType);
                }
            }

            return services;
        }
    }
}