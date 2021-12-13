using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreIocManager
{
    /// <summary>
    /// 配置Windsor注册模块
    /// </summary>
    public interface IWindsorContainerModule
    {
        void Configure(WindsorContainer container);
    }
}
