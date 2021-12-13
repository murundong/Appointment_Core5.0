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
using System.Threading.Tasks;

namespace CoreWeb_Appoint.Controllers
{
    public class ApplyCuratorController : ControllerBase
    {
        private IApplyCuratorService _applyCuratorService;
        private IUserInfoService _userInfoService;
        private IWeixinService _weixinService;
        private IWX_TOKEN_Service _tokenService;
        private ILogger<ApplyCuratorController> _logger;


        public ApplyCuratorController(IApplyCuratorService applyCuratorService, IUserInfoService userInfoService,
            IWeixinService weixinService, IWX_TOKEN_Service tokenService, ILogger<ApplyCuratorController> logger)
        {
            _applyCuratorService = applyCuratorService;
            _userInfoService = userInfoService;
            _weixinService = weixinService;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public IActionResult ApplyCurator(string openid, string msg)
        {
            if (!string.IsNullOrWhiteSpace(openid))
            {
                if (_applyCuratorService.GetApplyCurator(openid) != null)
                {
                    return ReturnJsonResult("请不要重复提交申请！", Enum_ReturnRes.Fail);
                }
                if (_applyCuratorService.CreateApply(new ApplyCurator() { apply_msg = msg, open_id = openid, }) != null)
                {
                    string token = GetNowToken();
                    Task.Run(() =>
                    {
                        try
                        {
                            var ApplyUser = _userInfoService.GetUinfoByOpenid(openid);

                            W_SUBS_DATA_INPUT data = new W_SUBS_DATA_INPUT()
                            {
                                touser = ConstConfig.SYS_OPENID,
                                access_token = token,
                                page = $"pages/CuratorApply/CuratorApply?oid={openid}",
                                template_id = ConstConfig.template_apply_curator,
                                data = new
                                {
                                    name1 = new { value = ApplyUser?.nick_name },
                                    thing5 = new { value = "馆主权限" },
                                    thing4 = new { value = $"{ (msg.Length > 15 ? (msg.Substring(0, 15) + "...") : msg)}" },
                                    date2 = new { value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                                },
                            };

                            var sendres = _weixinService.SendSubsCribe(data);
                            if (sendres.errCode != 0 && sendres.errCode != 43101)
                            {
                                _logger.LogError($"CancelTheCourse{sendres.errCode}_{sendres.errMsg}");
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    });
                }
            }
            return ReturnJsonResult();
        }


        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public IActionResult PassTheApply(string openid)
        {
            var ApplyModel = _applyCuratorService.GetApplyCurator(openid);
            if (ApplyModel != null)
            {
                ApplyModel.enum_status = Enum_ApplyCurator.pass;
                if (_applyCuratorService.UpdateBanners(ApplyModel))
                {
                    //更新role
                    var UserEntity = _userInfoService.GetUinfoByOpenid(openid);
                    if (_userInfoService.AllocRole(UserEntity.uid, Enum_UserRole.Curator))
                    {
                        Task.Run(() =>
                        {
                            string token = GetNowToken();
                            W_SUBS_DATA_INPUT data = new W_SUBS_DATA_INPUT()
                            {
                                touser = openid,
                                access_token = token,
                                page = "pages/mine/mine",
                                template_id = ConstConfig.template_agree_curator,
                                data = new { thing5 = new { value = "馆主权限" }, phrase6 = new { value = $"申请成功", }, },
                            };

                            var sendres = _weixinService.SendSubsCribe(data);
                            if (sendres.errCode != 0 && sendres.errCode != 43101)
                            {
                                _logger.LogError($"CancelTheCourse{sendres.errCode}_{sendres.errMsg}");
                            }
                        });
                    }
                }
            }
            return ReturnJsonResult();
        }

        public IActionResult GetApplyInfo(string openid)
        {
            View_ApplyUserInfo res = new View_ApplyUserInfo();
            if (!string.IsNullOrWhiteSpace(openid))
            {
                var U_Entity = _userInfoService.GetUinfoByOpenid(openid);
                var A_Entity = _applyCuratorService.GetApplyCurator(openid);
                res.open_id = openid;
                res.nick_name = U_Entity?.nick_name;
                res.avatar = U_Entity?.avatar;
                res.gender = U_Entity?.gender;
                res.tel = U_Entity?.tel;
                res.real_name = U_Entity?.real_name;
                res.birthday = U_Entity?.birthday;
                res.apply_msg = A_Entity?.apply_msg;
                res.apply_status = A_Entity?.enum_status;
            }
            return ReturnJsonResult(res);
        }
        #region MyRegion
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
        #endregion
    }
}
