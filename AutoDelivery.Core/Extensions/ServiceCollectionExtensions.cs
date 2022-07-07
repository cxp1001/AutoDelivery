using System.Reflection;
using System.Runtime.Loader;
using AutoDelivery.Domain;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace AutoDelivery.Core.Extensions
{
    public static class AutofacServiceCollectionExtensions
    {


        public static IServiceCollection AddModule(this IServiceCollection services, ContainerBuilder builder)
        {
            // 拿到与本项目有关联的项目
            var compileLibraries = DependencyContext.Default.CompileLibraries.Where(l=>!l.Serviceable&&l.Type!="package").ToList();
            // Where(l => !l.Serviceable && l.Type == "project").ToList();
            // 获取程序集
            List<Assembly> assemblies = new();
            foreach (var compileLibrary in compileLibraries)
            {
                assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(compileLibrary.Name)));
            }

            builder.RegisterAssemblyTypes(assemblies.ToArray()).Where(type => !type.IsAbstract && type.IsAssignableTo<IDependency>()).
            AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();

            return services;
        }


    }
}