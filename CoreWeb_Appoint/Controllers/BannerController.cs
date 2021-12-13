using CoreApplication.IWeixinApi;
using CoreApplication.Services.IServices;
using CoreEntityFramework;
using CoreEntityFramework.Enum;
using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using CoreEntityFramework.WeixData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
namespace CoreWeb_Appoint.Controllers
{
    public class BannerController: ControllerBase
    {
        private IBannerService _bannerService;
        private ILogger<BannerController> _logger;
        public BannerController(IBannerService bannerService, ILogger<BannerController> logger)
        {
            _bannerService = bannerService;
            _logger = logger;
        }
        [HttpPost]
        public IActionResult CreateBanner(Banners model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.img)) return ReturnJsonResult("新增失败，参数错误！", Enum_ReturnRes.Fail);
            var res = _bannerService.CreateBanners(model);
            if (res != null) return ReturnJsonResult(res);
            return ReturnJsonResult("新增失败！", Enum_ReturnRes.Fail);
        }


        [HttpPost]
        public IActionResult UpdateBanner(Banners model)
        {
            if (model == null || model.id <= 0 || string.IsNullOrWhiteSpace(model.img)) return ReturnJsonResult("新增失败，参数错误！", Enum_ReturnRes.Fail);
            if (_bannerService.UpdateBanners(model)) return ReturnJsonResult();
            return ReturnJsonResult("更新失败！", Enum_ReturnRes.Fail);
        }

        public IActionResult DeleteBanner(int? id)
        {
            if (!id.HasValue || id <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            if (!_bannerService.DeleteBanner((int)id)) return ReturnJsonResult("删除失败！", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }

        public IActionResult GetBannerItem(int? id)
        {
            if (!id.HasValue || id <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _bannerService.GetBannerById((int)id);
            return ReturnJsonResult(res);
        }

        //  [HttpPost]
        public IActionResult GetPageBanners(Base_PageInput input)
        {
            var res = _bannerService.PageBanners(input);
            return ReturnJsonResult(res);
        }
    }
}
