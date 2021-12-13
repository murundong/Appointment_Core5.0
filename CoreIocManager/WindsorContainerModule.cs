using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreIocManager
{
    public class WindsorContainerModule : IWindsorContainerModule
    {
        public void Configure(WindsorContainer container)
        {
            container.Install(new IRepositoryInstaller());
            container.Install(new IApplicationServiceInstaller());
            container.Install(new IAutoMapperInstaller());
        }
    }
}
