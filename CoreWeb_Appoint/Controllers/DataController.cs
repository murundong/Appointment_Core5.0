using CoreApplication.IWeixinApi;
using CoreApplication.Services.IServices;
using CoreBaseClass;
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
    public class DataController : ControllerBase
    {

        private IWeixinService _weixinService;
        private IUserInfoService _userInfoService;
        private IDoorUsersService _doorUserService;
        private IBannerService _bannerService;
        private IDoorsService _doorService;
        private ICardTemplateService _cardTemplateService;
        private ISubjectsService _subjectService;
        private ICoursesService _courseService;
        private IDoorUsersCardsService _doorUserCardService;
        private IDoorUsersAppointsService _doorUserAppointService;
        public DataController(
                IWeixinService weixinService,
                IUserInfoService userInfoService,
                IDoorUsersService doorUserService,
                IBannerService bannerService,
                IDoorsService doorService,
                ICardTemplateService cardTemplateService,
                ISubjectsService subjectService,
                ICoursesService courseService,
                IDoorUsersCardsService doorUserCardService,
                IDoorUsersAppointsService doorUserAppointService
            )
        {
            _weixinService = weixinService;
            _userInfoService = userInfoService;
            _doorUserService = doorUserService;
            _bannerService = bannerService;
            _doorService = doorService;
            _cardTemplateService = cardTemplateService;
            _subjectService = subjectService;
            _courseService = courseService;
            _doorUserCardService = doorUserCardService;
            _doorUserAppointService = doorUserAppointService;
        }

        public IActionResult GetOpenidByCode(string code)
        {
            var res = _weixinService.GetOpenIdByCode(code);
            return Json(res);
        }


        [HttpPost]
        public IActionResult UpdateUserInfoHome(UserInfos model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.nick_name)) model.initial = model.nick_name.GetInitial();
                if (string.IsNullOrWhiteSpace(model.open_id)) return ReturnJsonResult();
            }
            catch { }
            var res = _userInfoService.UpdateUserInfo_home(model);
            return ReturnJsonResult(res);
        }

        [HttpPost]
        public IActionResult UpdateUserInfoSetting(UserInfos model)
        {
            if (string.IsNullOrWhiteSpace(model.open_id)) return ReturnJsonResult();
            var res = _userInfoService.UpdateUserInfo_setting(model);
            return ReturnJsonResult(res);
        }


        public IActionResult GetUserInfoByOpenId(string openid)
        {
            if (string.IsNullOrWhiteSpace(openid)) return ReturnJsonResult("无效的参数", Enum_ReturnRes.Fail);
            var res = _userInfoService.GetUinfoByOpenid(openid);
            return ReturnJsonResult(res);
        }

        public IActionResult CheckUserHasManageMenu(int? uid)
        {
            if (!uid.HasValue) return ReturnJsonResult(false);
            var res = _doorUserService.CheckHasAdminMenu((int)uid);
            return ReturnJsonResult(res);
        }



        public IActionResult DecodeUserInfo(string signature, string encryptedData, string iv, string sk)
        {

            string session_key = sk;

            byte[] iv2 = Convert.FromBase64String(iv);

            if (string.IsNullOrEmpty(encryptedData)) return Json("");
            Byte[] toEncryptArray = Convert.FromBase64String(encryptedData);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Convert.FromBase64String(session_key),
                IV = iv2,
                Mode = System.Security.Cryptography.CipherMode.CBC,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Json(System.Text.Encoding.UTF8.GetString(resultArray));
        }


        #region MainPage
        public IActionResult GetBanners()
        {
            var res = _bannerService.GetBanners();
            return ReturnJsonResult(res);
        }

        [HttpPost]
        public IActionResult GetDoors(View_DoorInput input)
        {
            var res = _doorService.GetDoors(input);
            return ReturnJsonResult(res);
        }

        public IActionResult GetDoorsById(int? doorid)
        {
            if (doorid == null || doorid <= 0) return ReturnJsonResult("参数错误", Enum_ReturnRes.Fail);
            var res = _doorService.GetDoorsById((int)doorid);
            return ReturnJsonResult(res);
        }

        [HttpPost]
        public IActionResult GetTeacherDoors(View_TeacherDoorInput input)
        {
            var res = _doorService.GetTeacherDoors(input);
            return ReturnJsonResult(res);
        }

        [HttpPost]
        public IActionResult GetAdminAllDoors(Base_PageInput input)
        {
            var res = _doorService.GetAdminAllDoors(input);
            return ReturnJsonResult(res);
        }

        [HttpPost]
        public IActionResult CreateDoors(Doors model)
        {
            var res = _doorService.CreateDoors(model);
            if (res != null) return ReturnJsonResult(res);
            return ReturnJsonResult("创建失败！", Enum_ReturnRes.Fail);
        }

        [HttpPost]
        public IActionResult UpdateDoors(Doors model)
        {
            var res = _doorService.UpdateDoors(model);
            if (res) return ReturnJsonResult();
            return ReturnJsonResult("更新失败！", Enum_ReturnRes.Fail);
        }

        public IActionResult CheckUserBlack(string openid, int? doorid)
        {
            var res = _doorUserService.CheckUserBlackList(openid, (int)doorid);
            return ReturnJsonResult(res);
        }
        #endregion


        #region CardTemplate
        [HttpPost]
        public IActionResult GetCardTempalte(View_CardTemplateInput input)
        {
            var res = _cardTemplateService.PageCardTemplate(input);
            return ReturnJsonResult(res);
        }

        public IActionResult GetDoorCardSelect(int? doorId)
        {
            if (doorId == null || doorId <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _cardTemplateService.GetDoorCards((int)doorId);
            return ReturnJsonResult(res);
        }

        public IActionResult GetCardTemplateById(int? cardId)
        {
            if (cardId == null || cardId <= 0) return ReturnJsonResult("参数错误", Enum_ReturnRes.Fail);
            var res = _cardTemplateService.GetCardsTemplateById((int)cardId);
            return ReturnJsonResult(res);
        }

        [HttpPost]
        public IActionResult CreateCardTempalte(CardTemplate model)
        {

            var res = _cardTemplateService.CreateCardTemplate(model);
            if (res != null) return ReturnJsonResult(res);
            return ReturnJsonResult("创建失败！", Enum_ReturnRes.Fail);
        }

        [HttpPost]
        public IActionResult UpdateCardtemplate(CardTemplate model)
        {
            var res = _cardTemplateService.UpdateCardTemplate(model);
            if (res) return ReturnJsonResult();
            return ReturnJsonResult("更新失败！", Enum_ReturnRes.Fail);
        }
        #endregion

        #region Subjects
        [HttpPost]
        public IActionResult GetSubjects(View_SubjectsInput input)
        {
            var res = _subjectService.GetSubjects(input);
            return ReturnJsonResult(res);
        }



        [HttpPost]
        public IActionResult CreateSubject(Subjects model)
        {
            var res = _subjectService.CreateSubject(model);
            if (res != null) return ReturnJsonResult(res);
            return ReturnJsonResult("创建失败！", Enum_ReturnRes.Fail);
        }


        public IActionResult GetSubjectById(int id)
        {
            var res = _subjectService.GetSubjectById(id);
            return ReturnJsonResult(res);
        }

        [HttpPost]
        public IActionResult UpdateSubject(Subjects model)
        {
            var res = _subjectService.UpdateSubject(model);
            if (res) return ReturnJsonResult();
            return ReturnJsonResult("更新失败！", Enum_ReturnRes.Fail);
        }
        #endregion

        #region Lessons

        public IActionResult GetDoorInfo(int? doorid)
        {
            if (doorid == null || doorid <= 0) return ReturnJsonResult("参数错误", Enum_ReturnRes.Fail);
            var res = _doorService.GetDoorInfo((int)doorid);
            return ReturnJsonResult(res);
        }

        #endregion

        #region Course

        [HttpPost]
        public IActionResult GetAdminCourseByDate(View_CoursesInput input)
        {
            var res = _courseService.GetCourses(input);
            return ReturnJsonResult(res);
        }

        [HttpPost]
        public IActionResult CreateCourse(Courses model)
        {
            var res = _courseService.CreateCourse(model);
            if (res != null) return ReturnJsonResult(res);
            return ReturnJsonResult("新增失败", Enum_ReturnRes.Fail);
        }


        public IActionResult GetCourseById(int id)
        {
            var res = _courseService.GetCourseById(id);
            return ReturnJsonResult(res);
        }

        public IActionResult GetAddCourseData(int? doorId)
        {
            if (doorId == null || doorId <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            View_AddCourseData res = new View_AddCourseData();
            res.subjects = _subjectService.GetSubjectsByDoorID((int)doorId);
            res.cards = _cardTemplateService.GetDoorCards((int)doorId);
            return ReturnJsonResult(res);
        }

        [HttpPost]
        public IActionResult UpdateCourse(Courses model)
        {
            if (_courseService.UpdateCourse(model))
                return ReturnJsonResult();
            return ReturnJsonResult("更新失败！", Enum_ReturnRes.Fail);
        }

        public IActionResult QuickCourse(string sdate, string cdate, int? doorid, string openid)
        {
            if (string.IsNullOrWhiteSpace(sdate) || string.IsNullOrWhiteSpace(cdate) || string.IsNullOrWhiteSpace(openid) || !doorid.HasValue)
                return ReturnJsonResult("操作失败,缺少参数!", Enum_ReturnRes.Fail);
            if (!_courseService.QuickCourse(sdate, cdate, (int)doorid, openid)) return ReturnJsonResult("操作失败或没有可复制的课程！", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }

        public IActionResult DeleteCourse(int? cid)
        {
            if (!cid.HasValue) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            if (!_courseService.DeleteCourse((int)cid)) return ReturnJsonResult("删除失败！", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }


        [HttpPost]
        public IActionResult GetWeekCourse(View_WeekCourseInput input)
        {
            var res = _courseService.GetWeekCourse(input);
            return ReturnJsonResult(res);
        }

        #endregion

        #region Member

        public IActionResult RemarkUser(int? uid, string rmk, bool isMain = true)
        {
            if (!uid.HasValue || uid <= 0) return ReturnJsonResult("备注失败，参数错误！", Enum_ReturnRes.Fail);
            if (isMain)
            {
                if (!_userInfoService.SetUSerRemark((int)uid, rmk))
                {
                    return ReturnJsonResult("备注失败！", Enum_ReturnRes.Fail);
                }
            }
            else
            {
                if (!_doorUserService.SetUserRemark((int)uid, rmk))
                {
                    return ReturnJsonResult("备注失败！", Enum_ReturnRes.Fail);
                }
            }
            return ReturnJsonResult();
        }

        public IActionResult AddUserAttention(string openid, int? doorid)
        {
            if (string.IsNullOrWhiteSpace(openid) || !doorid.HasValue) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            _doorUserService.AddUserAttention(openid, (int)doorid);
            return ReturnJsonResult();
        }

        public IActionResult AllocRole(int? uid, Enum_UserRole? role, bool isMain = true)
        {
            if (uid == null || uid <= 0 || !role.HasValue) return ReturnJsonResult("更新失败，参数错误!", Enum_ReturnRes.Fail);
            if (isMain)
            {
                if (!_userInfoService.AllocRole((int)uid, (Enum_UserRole)role)) return ReturnJsonResult("分配失败！", Enum_ReturnRes.Fail);
            }
            else
            {
                if (!_doorUserService.AllocRole((int)uid, (Enum_UserRole)role)) return ReturnJsonResult("分配失败！", Enum_ReturnRes.Fail);
            }
            return ReturnJsonResult();
        }

        public IActionResult GetUserLst_Admin(string nick)
        {
            var res = _userInfoService.GetUserLst_Admin(nick);
            return ReturnJsonResult(res);
        }

        public IActionResult GetUserLst_Door(int? doorid, string nick)
        {
            if (!doorid.HasValue || doorid <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _doorUserCardService.GetUserLst_Door((int)doorid, nick);
            return ReturnJsonResult(res);
        }
        public IActionResult GetUserLst_SelfAppint(int? doorid, int? course_id, string nick)
        {
            if (!doorid.HasValue || doorid <= 0 || !course_id.HasValue || course_id <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _doorUserCardService.GetUserLst_SelfAppint((int)doorid, (int)course_id, nick);
            return ReturnJsonResult(res);
        }

        #endregion

        #region Cards
        public IActionResult GetDoorCardTemplates(int? doorId)
        {
            if (!doorId.HasValue || doorId <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _cardTemplateService.GetAllDoorCardsTemplate((int)doorId);
            return ReturnJsonResult(res);
        }


        public IActionResult GetUserCards(string openid, Enum_CardStatus cardStatus = Enum_CardStatus.All)
        {
            var res = _doorUserCardService.GetUserALlCards(openid, cardStatus);
            return ReturnJsonResult(res);
        }

        public IActionResult GetUserDoorCards(int? uid, int? doorId)
        {
            if (!uid.HasValue || !doorId.HasValue) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _doorUserCardService.GetUserDoorCards((int)uid, (int)doorId);
            return ReturnJsonResult(res);
        }

        public IActionResult GetDoorUserInfo(int? id)
        {
            if (!id.HasValue) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _doorUserService.GetDoorUserInfo((int)id);
            return ReturnJsonResult(res);
        }


        public IActionResult AddUserACard(DoorUsersCards model)
        {
            if (model.du_id <= 0 || model.cid <= 0 || !model.ctype.HasValue || model.uid <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            if (!_doorUserCardService.AddUserCards(model)) return ReturnJsonResult("操作失败！", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }

        public IActionResult GetUserCardsInfo(int? id)
        {
            if (!id.HasValue) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _doorUserCardService.GetUserCardsInfo(id);
            return ReturnJsonResult(res);
        }

        public IActionResult UpdateUserCardsInfo(DoorUsersCards model)
        {
            if (model.id <= 0 || model.cid <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            if (!_doorUserCardService.UpdateUserCardsInfo(model)) return ReturnJsonResult("操作失败！", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }

        public IActionResult DeleteUserCards(int? id)
        {
            if (!id.HasValue) return ReturnJsonResult("操作失败,参数错误！", Enum_ReturnRes.Fail);
            if (!_doorUserCardService.DeleteUserCards(id)) return ReturnJsonResult("操作失败！", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }

        #endregion

        public IActionResult GetUserStatistic(int? uid)
        {
            if (!uid.HasValue) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _doorUserAppointService.GetUserStatistic((int)uid);
            return ReturnJsonResult(res);
        }

    }
}
