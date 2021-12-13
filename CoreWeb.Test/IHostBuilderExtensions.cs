using Castle.Windsor;
using CoreIocManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWeb.Test
{
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder UserWindsorContainer<TWindsorContainerModule>(this IHostBuilder hostBuilder,
            TWindsorContainerModule containerModule)
            where TWindsorContainerModule:class, IWindsorContainerModule
        {
            return hostBuilder
                .UseServiceProviderFactory(new WindsorContainerFactory(containerModule, new WindsorContainer()))
                .ConfigureServices(service =>
                {
                    service.AddSingleton<IWindsorContainerModule, TWindsorContainerModule>();
                });

        }
    }
}
