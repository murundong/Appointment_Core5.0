using CoreApplication.IWeixinApi;
using CoreApplication.Services.IServices;
using CoreEntityFramework;
using CoreEntityFramework.Enum;
using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using CoreEntityFramework.WeixData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWeb_Appoint.Controllers
{
    public class AppointController : ControllerBase
    {
        private IDoorUsersCardsService _doorUserCardService;
        private ICoursesService _courseService;
        private IDoorUsersAppointsService _doorUserAppointService;
        private IDoorUsersQueueAppointsService _doorUserQueueAppointService;
        private ISubjectsService _subjectService;
        private IWeixinService _weixinService;
        private IDoorUsersCourseCommentsService _doorUserCourseCommentService;
        private IWX_TOKEN_Service _tokenService;
        private ILogger<AppointController> _logger;


        public AppointController(
            IDoorUsersCardsService doorUserCardService,
        ICoursesService courseService,
        IDoorUsersAppointsService doorUserAppointService,
        IDoorUsersQueueAppointsService doorUserQueueAppointService,
        ISubjectsService subjectService,
        IWeixinService weixinService,
        IDoorUsersCourseCommentsService doorUserCourseCommentService,
        IWX_TOKEN_Service tokenService,
        ILogger<AppointController> logger
            )
        {
            _doorUserCardService = doorUserCardService;
            _courseService = courseService;
            _doorUserAppointService = doorUserAppointService;
            _doorUserQueueAppointService = doorUserQueueAppointService;
            _subjectService = subjectService;
            _weixinService = weixinService;
            _doorUserCourseCommentService = doorUserCourseCommentService;
            _tokenService = tokenService;
            _logger = logger;
        }


        public IActionResult GetUserCanUseDoorCards(int? doorId, int? uid)
        {
            if (!doorId.HasValue || doorId <= 0 || !uid.HasValue || uid <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            List<View_Appoint_UsersCardsInfo> return_res = _doorUserCardService.GetAppointDoorUserCardsInfo(doorId, uid);
            List<View_Appoint_UserCanUseCardsOutput> res = new List<View_Appoint_UserCanUseCardsOutput>();
            if (return_res != null && return_res.Count > 0)
            {
                return_res.ForEach(s =>
                {
                    if (s.card_sttime.HasValue && s.card_sttime <= DateTime.Now &&
                    ((s.card_edtime.HasValue && s.card_edtime > DateTime.Now) || !s.card_edtime.HasValue) && !s.is_freeze)
                    {
                        res.Add(new View_Appoint_UserCanUseCardsOutput() { id = s.id, card_name = s.card_name });
                    }
                });
            }
            return ReturnJsonResult(res);
        }

        public IActionResult CheckCourseNeedCards(int? course_id)
        {
            if (!course_id.HasValue) return ReturnJsonResult(false);
            var res = _courseService.CheckCourseNeedCards((int)course_id);
            return ReturnJsonResult(res);
        }
        public IActionResult GetAppointLessons(View_GetAppointCourseInput input)
        {
            if (input == null || input.doorId <= 0 || string.IsNullOrWhiteSpace(input.date)) ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            if (input.tag == "全部课程") input.tag = null;
            var res = _courseService.GetDoorAppointCourse(input);
            if (res != null && res.total > 0)
            {
                res.data.ForEach(s =>
                {
                    s.AppointStatus = _doorUserAppointService.GetCourseAppointStatus(input.uid, s.id);
                    if (s.AppointStatus == Enum_AppointStatus.SHOW_CANCEL)
                    {
                        //判断是否允许取消
                        if (!_courseService.CheckCourseCanCancel(s.id)) s.AppointStatus = Enum_AppointStatus.SHOW_NOTCANCEL;
                    }
                    else if (s.AppointStatus == Enum_AppointStatus.SHOW_QUEUE)
                    {
                        if (_doorUserQueueAppointService.CheckUserAlreadyQuee(input.uid, s.id)) s.AppointStatus = Enum_AppointStatus.SHOW_CANCEL_QUEUE;
                    }
                });
            }
            return ReturnJsonResult(res);
        }

        public IActionResult GetDoorTags(int? doorId)
        {
            if (!doorId.HasValue || doorId <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _subjectService.GetDoorTags(doorId);
            if (res.Count > 0) res.Insert(0, "全部课程");
            return ReturnJsonResult(res);
        }

        public IActionResult AppointCourse(View_AppointCourseInput input)
        {
            if (input.course_id <= 0 || input.uid <= 0) return ReturnJsonResult("预约失败！参数错误！", Enum_ReturnRes.Fail);
            //校验课程
            Enum_AppointStatus status = _doorUserAppointService.GetCourseAppointStatus(input.uid, input.course_id);
            switch (status)
            {
                case Enum_AppointStatus.SHOW_ONLYTODY:
                    return ReturnJsonResult("该课程仅允许当日预约！", Enum_ReturnRes.Fail);
                case Enum_AppointStatus.SHOW_FULLPEOPLE:
                case Enum_AppointStatus.SHOW_QUEUE:
                    return ReturnJsonResult("该课程已经约满了哦！", Enum_ReturnRes.Fail);
                case Enum_AppointStatus.SHOW_TIMEEXPIRED:
                    return ReturnJsonResult("现在不在可约课时间段哦！", Enum_ReturnRes.Fail);
                case Enum_AppointStatus.SHOW_NULL:
                    return ReturnJsonResult("暂时不可以预约哦", Enum_ReturnRes.Fail);
                case Enum_AppointStatus.SHOW_CANCEL:
                    return ReturnJsonResult("请不要重复预约哦！", Enum_ReturnRes.Fail);
                default:
                    break;
            }
            if (status != Enum_AppointStatus.SHOW_APPOINT) return ReturnJsonResult("暂时不可以预约哦", Enum_ReturnRes.Fail);
            //课程是否需要会员卡
            var card_need = _courseService.GetCourseCards(input.course_id);
            if (card_need != null && card_need.Count > 0)
            {
                if (!input.card_id.HasValue) return ReturnJsonResult("没有选择可用的会员卡！", Enum_ReturnRes.Fail);
                //获取会员卡模板
                int template_cardid = _doorUserCardService.GetCardTempalteId((int)input.card_id);
                if (input.card_id <= 0 || !card_need.Contains(template_cardid)) return ReturnJsonResult("没有选择可用的会员卡！", Enum_ReturnRes.Fail);
                //判断会员卡是否可用(次数、时间、冻结等)
                if (!_doorUserCardService.CheckCardsCanUse((int)input.card_id)) return ReturnJsonResult("选择的会员卡不可用！", Enum_ReturnRes.Fail);
                //每周/每日限制
                if (!_doorUserCardService.CheckCardLimitTimes((int)input.uid, (int)input.card_id)) return ReturnJsonResult("该会员卡达到 每日/周 预约上限！", Enum_ReturnRes.Fail);
                //扣卡
                _doorUserCardService.DeductionUserCards((int)input.card_id);
            }
            if (!_doorUserAppointService.CheckUserExsitBindding(input.door_id, input.uid)) return ReturnJsonResult("请先登录！", Enum_ReturnRes.Fail);
            if (!_doorUserAppointService.AddUserAppoint(input.uid, input.door_id, input.course_id, input.card_id)) return ReturnJsonResult("预约失败，请稍后尝试！", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }

        public IActionResult SelfAppointCourse(View_AppointCourseInput input)
        {
            if (input.course_id <= 0 || input.uid <= 0) return ReturnJsonResult("自助预约失败！参数错误！", Enum_ReturnRes.Fail);
            //校验课程
            Enum_AppointStatus status = _doorUserAppointService.GetCourseAppointStatus(input.uid, input.course_id);
            if (status == Enum_AppointStatus.SHOW_CANCEL) return ReturnJsonResult("自助预约失败，该用户已预约了该课程！", Enum_ReturnRes.Fail);
            //switch (status)
            //{
            //    case Enum_AppointStatus.SHOW_ONLYTODY:
            //        return ReturnJsonResult("该课程仅允许当日预约！", Enum_ReturnRes.Fail);
            //    case Enum_AppointStatus.SHOW_FULLPEOPLE:
            //    case Enum_AppointStatus.SHOW_QUEUE:
            //        return ReturnJsonResult("该课程已经约满了哦！", Enum_ReturnRes.Fail);
            //    case Enum_AppointStatus.SHOW_TIMEEXPIRED:
            //        return ReturnJsonResult("现在不在可约课时间段哦！", Enum_ReturnRes.Fail);
            //    case Enum_AppointStatus.SHOW_NULL:
            //        return ReturnJsonResult("暂时不可以预约哦", Enum_ReturnRes.Fail);
            //    case Enum_AppointStatus.SHOW_CANCEL:
            //        return ReturnJsonResult("请不要重复预约哦！", Enum_ReturnRes.Fail);
            //    default:
            //        break;
            //}
            //if (status != Enum_AppointStatus.SHOW_APPOINT) return ReturnJsonResult("暂时不可以预约哦", Enum_ReturnRes.Fail);
            //课程是否需要会员卡
            var card_need = _courseService.GetCourseCards(input.course_id);
            if (card_need != null && card_need.Count > 0)
            {
                if (!input.card_id.HasValue) return ReturnJsonResult("没有选择可用的会员卡！", Enum_ReturnRes.Fail);
                //获取会员卡模板
                int template_cardid = _doorUserCardService.GetCardTempalteId((int)input.card_id);
                if (input.card_id <= 0 || !card_need.Contains(template_cardid)) return ReturnJsonResult("没有选择可用的会员卡！", Enum_ReturnRes.Fail);
                //判断会员卡是否可用(次数、时间、冻结等)
                if (!_doorUserCardService.CheckCardsCanUse((int)input.card_id)) return ReturnJsonResult("选择的会员卡不可用！", Enum_ReturnRes.Fail);
                //每周/每日限制
                if (!_doorUserCardService.CheckCardLimitTimes((int)input.uid, (int)input.card_id)) return ReturnJsonResult("该会员卡达到 每日/周 预约上限！", Enum_ReturnRes.Fail);
                //扣卡
                _doorUserCardService.DeductionUserCards((int)input.card_id);
            }
            if (!_doorUserAppointService.AddUserAppoint(input.uid, input.door_id, input.course_id, input.card_id)) return ReturnJsonResult("预约失败，请稍后尝试！", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }


        public IActionResult CancelAppointCourse(int? uid, int? courseid)
        {
            if (!uid.HasValue || uid <= 0 || !courseid.HasValue || courseid <= 0) return ReturnJsonResult("取消失败,请稍后重试！", Enum_ReturnRes.Fail);
            //已经取消
            if (_doorUserAppointService.IsUserAlreadyCancel(uid, courseid)) return ReturnJsonResult("请不要处重复取消！", Enum_ReturnRes.Fail);
            //取消时间限制
            if (!_courseService.CheckCourseCanCancel((int)courseid)) return ReturnJsonResult("取消失败,已经超过了该课程允许取消时间！", Enum_ReturnRes.Fail);
            //归还次数
            _doorUserCardService.RebackUserCards(uid, courseid);
            //取消课程
            if (!_doorUserAppointService.CancelAppoint(uid, courseid)) return ReturnJsonResult("取消失败,请稍后重试！", Enum_ReturnRes.Fail);
            //有排队的处理队列
            Task.Run(() =>
            {
                ProcessQueue((int)courseid);
            });
            return ReturnJsonResult();
        }

        public IActionResult QuitAppointCards(int? uid, int? courseid)
        {
            if (!uid.HasValue || uid <= 0 || !courseid.HasValue || courseid <= 0) return ReturnJsonResult("取消失败,请稍后重试！", Enum_ReturnRes.Fail);
            //已经取消
            if (_doorUserAppointService.IsUserAlreadyCancel(uid, courseid)) return ReturnJsonResult("该用户已自行取消课程，无需操作！", Enum_ReturnRes.Fail);
            //归还次数
            _doorUserCardService.RebackUserCards(uid, courseid);
            //退课
            if (!_doorUserAppointService.ReturnAppoint(uid, courseid)) return ReturnJsonResult("取消失败,请稍后重试！", Enum_ReturnRes.Fail);
            //有排队的处理队列
            Task.Run(() =>
            {
                //退课通知，暂时不做
                //try
                //{
                //    W_SUBS_DATA_INPUT data = new W_SUBS_DATA_INPUT()
                //    {
                //        touser = s.open_id,
                //        access_token = token,
                //        page = $"pages/Lesson/Lesson?doorId={s.door_id}&doorName={s.door_name}",
                //        template_id = ConstConfig.template_cancel,
                //        data = new { thing6 = new { value = s.subject_title }, date2 = new { value = s.course_date + " " + s.course_time }, thing4 = new { value = "场馆工作人员手动取消" } }
                //    };
                //    var sendres = _weixinService.SendSubsCribe(data);
                //    if (sendres.errCode != 0 && sendres.errCode != 43101)
                //    {
                //        Log.Error($"CancelTheCourse{sendres.errCode}_{sendres.errMsg}");
                //    }
                //}
                //catch (Exception ex)
                //{
                //}
                ProcessQueue((int)courseid);
            });




            return ReturnJsonResult();
        }


        public IActionResult CancelQueue(int? uid, int? courseid)
        {
            if (!uid.HasValue || uid <= 0 || !courseid.HasValue || courseid <= 0) return ReturnJsonResult("取消失败,请稍后重试！", Enum_ReturnRes.Fail);
            if (!_doorUserQueueAppointService.CheckUserAlreadyQuee((int)uid, (int)courseid)) return ReturnJsonResult("取消失败，请先进行排队预约！", Enum_ReturnRes.Fail);
            if (!_doorUserQueueAppointService.CancelQueue((int)uid, (int)courseid)) return ReturnJsonResult("取消失败,请稍后重试！", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }

        public IActionResult QueueAppointCourse(View_AppointCourseInput input)
        {
            if (input.course_id <= 0 || input.uid <= 0) return ReturnJsonResult("排队预约失败！参数错误！", Enum_ReturnRes.Fail);
            //是否已经排队了
            if (_doorUserQueueAppointService.CheckUserAlreadyQuee(input.uid, input.course_id)) return ReturnJsonResult("请不要重复操作！", Enum_ReturnRes.Fail);
            //校验课程
            Enum_AppointStatus status = _doorUserAppointService.GetCourseAppointStatus(input.uid, input.course_id);
            switch (status)
            {
                case Enum_AppointStatus.SHOW_ONLYTODY:
                    return ReturnJsonResult("该课程仅允许当日预约！", Enum_ReturnRes.Fail);
                case Enum_AppointStatus.SHOW_FULLPEOPLE:
                    return ReturnJsonResult("该课程已经约满，且不支持排队预约哦！", Enum_ReturnRes.Fail);
                case Enum_AppointStatus.SHOW_TIMEEXPIRED:
                    return ReturnJsonResult("现在不在可约课时间段哦！", Enum_ReturnRes.Fail);
                case Enum_AppointStatus.SHOW_NULL:
                    return ReturnJsonResult("暂时不可以预约哦", Enum_ReturnRes.Fail);
                case Enum_AppointStatus.SHOW_CANCEL:
                    return ReturnJsonResult("请不要重复预约哦！", Enum_ReturnRes.Fail);
                default:
                    break;
            }
            if (status != Enum_AppointStatus.SHOW_QUEUE) return ReturnJsonResult("暂时不可以排队预约哦", Enum_ReturnRes.Fail);
            var card_need = _courseService.GetCourseCards(input.course_id);
            if (card_need != null && card_need.Count > 0)
            {
                if (!input.card_id.HasValue) return ReturnJsonResult("没有选择可用的会员卡！", Enum_ReturnRes.Fail);
                //获取会员卡模板
                int template_cardid = _doorUserCardService.GetCardTempalteId((int)input.card_id);
                if (input.card_id <= 0 || !card_need.Contains(template_cardid)) return ReturnJsonResult("没有选择可用的会员卡！", Enum_ReturnRes.Fail);
                //判断会员卡是否可用(次数、时间、冻结等)
                if (!_doorUserCardService.CheckCardsCanUse((int)input.card_id)) return ReturnJsonResult("选择的会员卡不可用！", Enum_ReturnRes.Fail);
                //每周/每日限制
                if (!_doorUserCardService.CheckCardLimitTimes((int)input.uid, (int)input.card_id)) return ReturnJsonResult("该会员卡达到 每日/周 预约上限！", Enum_ReturnRes.Fail);

            }
            if (!_doorUserQueueAppointService.Queue(input)) return ReturnJsonResult("操作失败，请稍后尝试", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }

        /// <summary>
        /// 教师取消该课程
        /// </summary>
        /// <param name="courseid"></param>
        /// <returns></returns>
        public IActionResult CancelTheCourse(int? courseid)
        {
            if (!courseid.HasValue || courseid <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            if (!_doorUserAppointService.CancselCourse((int)courseid)) return ReturnJsonResult("操作失败！", Enum_ReturnRes.Fail);
            var lst_users = _courseService.GetAllCourse((int)courseid);
            if (lst_users != null && lst_users.Count > 0)
            {
                string token = GetNowToken();
                Task.Run(() =>
                {
                    try
                    {
                        lst_users.ForEach(s =>
                        {
                            W_SUBS_DATA_INPUT data = new W_SUBS_DATA_INPUT()
                            {
                                touser = s.open_id,
                                access_token = token,
                                page = $"pages/Lesson/Lesson?doorId={s.door_id}&doorName={s.door_name}",
                                template_id = ConstConfig.template_cancel,
                                data = new { thing6 = new { value = s.subject_title }, date2 = new { value = s.course_date + " " + s.course_time }, thing4 = new { value = "场馆工作人员手动取消" } }
                            };
                            var sendres = _weixinService.SendSubsCribe(data);
                            if (sendres.errCode != 0 && sendres.errCode != 43101)
                            {
                                _logger.LogError($"CancelTheCourse{sendres.errCode}_{sendres.errMsg}");
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                    }
                });
            }
            return ReturnJsonResult();
        }


        public IActionResult GetMyAppointWait(View_MyAppointWaitInput input)
        {
            if (input?.uid <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _doorUserAppointService.GetMyAppointWait(input);
            if (res != null && res.data.Count > 0)
            {
                res.data.ForEach(s =>
                {
                    if (!_courseService.CheckCourseCanCancel(s.course_id)) s.AppointStatus = Enum_AppointStatus.SHOW_NOTCANCEL;
                });
            }
            return ReturnJsonResult(res);
        }
        public IActionResult GetMyAppointComp(View_MyAppointWaitInput input)
        {
            if (input?.uid <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _doorUserAppointService.GetMyAppointComp(input);
            return ReturnJsonResult(res);
        }

        public IActionResult GetJudgeCourseInfo(int? cid)
        {
            if (!cid.HasValue) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _courseService.GetJudgeCourseInfo(cid);
            return ReturnJsonResult(res);
        }

        public IActionResult JudgeCourse(DoorUsersCourseComments input)
        {
            if (input.uid <= 0 || input.course_id <= 0) return ReturnJsonResult("参数错误!", Enum_ReturnRes.Fail);
            if (!_doorUserCourseCommentService.AddJudge(input)) return ReturnJsonResult("评论失败，请稍后重试~", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }

        public IActionResult QrCode(int? cid)
        {
            if (!cid.HasValue && cid <= 0)
            {
                return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            }
            string text = $"{{\"cid\":{cid}}}";
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.L);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            MemoryStream ms = new MemoryStream();
            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            qrCodeImage.Dispose();
            return File(ms.ToArray(), "image/jpeg");
        }

        public IActionResult GetSignCourse(int? cid)
        {
            if (!cid.HasValue || cid <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _courseService.GetSignCourseById((int)cid);
            return ReturnJsonResult(res);
        }


        public IActionResult SignAppointCourse(View_AppointCourseInput input)
        {
            if (input.course_id <= 0 || input.uid <= 0) return ReturnJsonResult("预约失败！参数错误！", Enum_ReturnRes.Fail);
            //校验课程
            Enum_AppointStatus status = _doorUserAppointService.GetCourseAppointStatus(input.uid, input.course_id);
            if (status == Enum_AppointStatus.SHOW_CANCEL) return ReturnJsonResult("请不要重复预约统一节课程！", Enum_ReturnRes.Fail);
            //switch (status)
            //{
            //    case Enum_AppointStatus.SHOW_ONLYTODY:
            //        return ReturnJsonResult("该课程仅允许当日预约！", Enum_ReturnRes.Fail);
            //    case Enum_AppointStatus.SHOW_FULLPEOPLE:
            //    case Enum_AppointStatus.SHOW_QUEUE:
            //        return ReturnJsonResult("该课程已经约满了哦！", Enum_ReturnRes.Fail);
            //    case Enum_AppointStatus.SHOW_TIMEEXPIRED:
            //        return ReturnJsonResult("现在不在可约课时间段哦！", Enum_ReturnRes.Fail);
            //    case Enum_AppointStatus.SHOW_NULL:
            //        return ReturnJsonResult("暂时不可以预约哦", Enum_ReturnRes.Fail);
            //    case Enum_AppointStatus.SHOW_CANCEL:
            //        return ReturnJsonResult("请不要重复预约哦！", Enum_ReturnRes.Fail);
            //    default:
            //        break;
            //}
            //if (status != Enum_AppointStatus.SHOW_APPOINT) return ReturnJsonResult("暂时不可以预约哦", Enum_ReturnRes.Fail);
            //课程是否需要会员卡
            var card_need = _courseService.GetCourseCards(input.course_id);
            if (card_need != null && card_need.Count > 0)
            {
                if (!input.card_id.HasValue) return ReturnJsonResult("没有选择可用的会员卡！", Enum_ReturnRes.Fail);
                //获取会员卡模板
                int template_cardid = _doorUserCardService.GetCardTempalteId((int)input.card_id);
                if (input.card_id <= 0 || !card_need.Contains(template_cardid)) return ReturnJsonResult("没有选择可用的会员卡！", Enum_ReturnRes.Fail);
                //判断会员卡是否可用(次数、时间、冻结等)
                if (!_doorUserCardService.CheckCardsCanUse((int)input.card_id)) return ReturnJsonResult("选择的会员卡不可用！", Enum_ReturnRes.Fail);
                //每周/每日限制
                if (!_doorUserCardService.CheckCardLimitTimes((int)input.uid, (int)input.card_id)) return ReturnJsonResult("该会员卡达到 每日/周 预约上限！", Enum_ReturnRes.Fail);
                //扣卡
                _doorUserCardService.DeductionUserCards((int)input.card_id);
            }
            if (!_doorUserAppointService.AddUserAppoint(input.uid, input.door_id, input.course_id, input.card_id)) return ReturnJsonResult("预约失败，请稍后尝试！", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }


        public IActionResult GetAllCourse()
        {
            var res = _courseService.GetAllCourse();
            return ReturnJsonResult(res);
        }

        void ProcessQueue(int courseid)
        {
            try
            {
                var queueModel = _doorUserQueueAppointService.GetQueueUser(courseid);
                if (queueModel == null || queueModel.id <= 0) return;
                Enum_AppointStatus status = _doorUserAppointService.GetCourseAppointStatus(queueModel.uid, queueModel.course_id);
                if (status != Enum_AppointStatus.SHOW_APPOINT) return;
                var card_need = _courseService.GetCourseCards(queueModel.course_id);
                if (card_need != null && card_need.Count > 0)
                {
                    if (!queueModel.user_card_id.HasValue)
                    {
                        _doorUserQueueAppointService.DeleteUserQueue(queueModel);
                        ProcessQueue(courseid);
                    }
                    //获取会员卡模板
                    int template_cardid = _doorUserCardService.GetCardTempalteId((int)queueModel.user_card_id);
                    if (queueModel.user_card_id <= 0 || !card_need.Contains(template_cardid))
                    {
                        _doorUserQueueAppointService.DeleteUserQueue(queueModel);
                        ProcessQueue(courseid);
                    }
                    //判断会员卡是否可用(次数、时间、冻结等)
                    if (!_doorUserCardService.CheckCardsCanUse((int)queueModel.user_card_id))
                    {
                        _doorUserQueueAppointService.DeleteUserQueue(queueModel);
                        ProcessQueue(courseid);
                    }
                    //每周/每日限制
                    if (!_doorUserCardService.CheckCardLimitTimes(queueModel.uid, (int)queueModel.user_card_id))
                    {
                        _doorUserQueueAppointService.DeleteUserQueue(queueModel);
                        ProcessQueue(courseid);
                    }
                    //扣卡
                    _doorUserCardService.DeductionUserCards((int)queueModel.user_card_id);
                }
                _doorUserAppointService.CopyQueueAppoint(queueModel);
                _doorUserQueueAppointService.DeleteUserQueue(queueModel);
                //排队通知
                var notice_model = _doorUserQueueAppointService.GetQueenNoticDetail(queueModel.uid, courseid);
                if (notice_model != null && !string.IsNullOrWhiteSpace(notice_model.open_id))
                {
                    W_SUBS_DATA_INPUT data = new W_SUBS_DATA_INPUT()
                    {
                        touser = notice_model.open_id,
                        page = $"pages/appointment/appointment",
                        access_token = GetNowToken(),
                        template_id = ConstConfig.template_queue,
                        data = new { thing1 = new { value = notice_model.subject_title }, time3 = new { value = notice_model.course_date + " " + notice_model.course_time }, thing2 = new { value = notice_model.door_name }, thing5 = new { value = $"排队成功，请及时确认行程" } }
                    };
                    var sendres = _weixinService.SendSubsCribe(data);
                    if (sendres.errCode != 0 && sendres.errCode != 43101)
                    {
                        _logger.LogError($"ProcessQueue{sendres.errCode}_{sendres.errMsg}");
                    }
                }
            }
            catch (Exception ex)
            {
            }
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
