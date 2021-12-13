using CoreEntityFramework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.Services.IServices
{
    public interface IApplyCuratorService : IApplicationService
    {
        ApplyCurator CreateApply(ApplyCurator model);

        bool UpdateBanners(ApplyCurator model);

        ApplyCurator GetApplyCurator(string open_Id);
    }
}
