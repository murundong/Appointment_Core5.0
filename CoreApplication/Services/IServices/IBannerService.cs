using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.Services.IServices
{
   public interface IBannerService:IApplicationService
    {
        List<View_BannerOutput> GetBanners();

        Base_PageOutput<List<Banners>> PageBanners(Base_PageInput input);

        Banners CreateBanners(Banners model);

        bool UpdateBanners(Banners model);

        Banners GetBannerById(int id);

        bool DeleteBanner(int id);
    }
}
