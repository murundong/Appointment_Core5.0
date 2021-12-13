using CoreApplication.IWeixinApi;
using CoreApplication.Services.IServices;
using CoreEntityFramework;
using CoreEntityFramework.Enum;
using CoreEntityFramework.Model;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CoreWeb_Appoint.Controllers
{
    public class SubsMsgController : ControllerBase
    {
        private IDoorUsersSubsMsgService _subsMsgService;
        private IWX_TOKEN_Service _tokenService;
        private IWeixinService _weixinService;

        public SubsMsgController(
            IDoorUsersSubsMsgService subsMsgService,
            IWX_TOKEN_Service tokenService,
            IWeixinService weixinService)
        {
            _subsMsgService = subsMsgService;
            _tokenService = tokenService;
            _weixinService = weixinService;
        }

        public IActionResult AddUserSubs(DoorUsersSubsMsg model)
        {
            if (!_subsMsgService.AddData(model)) return ReturnJsonResult("订阅失败！", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }


        string GetNowToken()
        {
            string token = null, appid = ConstConfig.APPID;
            var TOKENITEM = _tokenService.GetToken(appid);
            if (TOKENITEM == null || string.IsNullOrWhiteSpace(TOKENITEM?.access_token) || TOKENITEM.create_time.AddSeconds(TOKENITEM.expires_in) <= DateTime.Now.AddMinutes(-10))
            {
                var WXTOKEN = _weixinService.GetToken();
                if (WXTOKEN != null && WXTOKEN.errcode == 0)
                {
                    token = WXTOKEN.access_token;
                    _tokenService.InsertOrUpdateToken(new WX_TOKEN() { appid = appid, access_token = WXTOKEN.access_token, expires_in = WXTOKEN.expires_in });
                }
            }
            else token = TOKENITEM.access_token;
            return token;
        }
    }
}
