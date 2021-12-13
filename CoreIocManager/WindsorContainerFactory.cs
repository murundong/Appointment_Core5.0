using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreIocManager
{
    /// <summary>
    /// 自定义DI容器
    /// https://www.cnblogs.com/NCoreCoder/p/13425603.html
    /// </summary>
    public class WindsorContainerFactory : IServiceProviderFactory<IServiceCollection>
    {
        //private WindsorContainer container;
        private IWindsorContainer _container;
        private IServiceCollection services;

        public WindsorContainerFactory(IWindsorContainerModule containerModule, IWindsorContainer container)
        {
            //container = new WindsorContainer();
            //containerModule.Configure(container);
            _container = container;
            containerModule.Configure(_container as WindsorContainer);
        }

        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            this.services = services;
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            //return WindsorRegistrationHelper.CreateServiceProvider(container, services);
            return WindsorRegistrationHelper.CreateServiceProvider(_container, services);
        }
    }
}
