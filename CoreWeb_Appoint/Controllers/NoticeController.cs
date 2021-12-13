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
    public class NoticeController : ControllerBase
    {
        private IDoorNoticeService _doorNoticeService;
        private IDoorUsersService _doorUserService;
        private INoticeService _noticeService;

        public NoticeController(
            IDoorNoticeService doorNoticeService,
            IDoorUsersService doorUserService,
            INoticeService noticeService)
        {
            _doorNoticeService = doorNoticeService;
            _doorUserService = doorUserService;
            _noticeService = noticeService;
        }

        [HttpPost]
        public IActionResult GetDoorNotice(View_DoorNoticeInput input)
        {
            var res = _doorNoticeService.GetDoorNotice(input);
            return ReturnJsonResult(res);
        }

        [HttpPost]
        public IActionResult CreateDoorNotice(DoorNotice model)
        {
            if (model != null && model.du_id > 0 && model.door_id > 0)
            {
                var item = _doorUserService.GetDoorUsersByUID(model.door_id, model.du_id);
                if (item != null) model.du_id = item.id;
                else return ReturnJsonResult("新增失败，参数错误！", Enum_ReturnRes.Fail);
            }
            else return ReturnJsonResult("新增失败，参数错误！", Enum_ReturnRes.Fail);
            var res = _doorNoticeService.CreateDoorNotice(model);
            if (res != null) return ReturnJsonResult(res);
            return ReturnJsonResult("新增失败！", Enum_ReturnRes.Fail);
        }

        [HttpPost]
        public IActionResult UpdateDoorNotice(DoorNotice model)
        {
            if (model != null && model.du_id > 0 && model.door_id > 0)
            {
                var item = _doorUserService.GetDoorUsersByUID(model.door_id, model.du_id);
                if (item != null) model.du_id = item.id;
                else return ReturnJsonResult("新增失败，参数错误！", Enum_ReturnRes.Fail);
            }
            else return ReturnJsonResult("新增失败，参数错误！", Enum_ReturnRes.Fail);
            if (_doorNoticeService.UpdateDoorNotice(model)) return ReturnJsonResult();
            return ReturnJsonResult("更新失败！", Enum_ReturnRes.Fail);
        }

        public IActionResult DeleteDoorNotice(int? id)
        {
            if (!id.HasValue || id <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            if (!_doorNoticeService.DeleteDoorNotice((int)id)) return ReturnJsonResult("删除失败！", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }

        public IActionResult GetDoorNoticeItem(int? id)
        {
            if (!id.HasValue || id <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _doorNoticeService.GetDoorNoticeItem((int)id);
            return ReturnJsonResult(res);
        }

        [HttpPost]
        public IActionResult GetNotice(View_NoticeInput input)
        {
            var res = _noticeService.GetNotice(input);
            return ReturnJsonResult(res);
        }

        [HttpPost]
        public IActionResult CreateNotice(Notice model)
        {
            if (model == null || model.uid <= 0 || string.IsNullOrWhiteSpace(model.title))
                return ReturnJsonResult("新增失败，参数错误！", Enum_ReturnRes.Fail);
            var res = _noticeService.CreateNotice(model);
            if (res != null) return ReturnJsonResult(res);
            return ReturnJsonResult("新增失败！", Enum_ReturnRes.Fail);
        }

        [HttpPost]
        public IActionResult UpdateNotice(Notice model)
        {
            if (model == null || model.uid <= 0 || string.IsNullOrWhiteSpace(model.title))
                return ReturnJsonResult("新增失败，参数错误！", Enum_ReturnRes.Fail);
            if (_noticeService.UpdateNotice(model)) return ReturnJsonResult();
            return ReturnJsonResult("更新失败！", Enum_ReturnRes.Fail);
        }

        public IActionResult DeleteNotice(int? id)
        {
            if (!id.HasValue || id <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            if (!_noticeService.DeleteNotice((int)id)) return ReturnJsonResult("删除失败！", Enum_ReturnRes.Fail);
            return ReturnJsonResult();
        }

        public IActionResult GetNoticeItem(int? id)
        {
            if (!id.HasValue || id <= 0) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _noticeService.GetNoticeItem((int)id);
            return ReturnJsonResult(res);
        }

        public IActionResult GetNewestNotice()
        {
            var res = _noticeService.GetNewestNotice();
            return ReturnJsonResult(res);
        }
        public IActionResult GetNewestDoorNotice(int? door_id)
        {
            if (!door_id.HasValue) return ReturnJsonResult("参数错误！", Enum_ReturnRes.Fail);
            var res = _doorNoticeService.GetDoorNewestNotice((int)door_id);
            return ReturnJsonResult(res);
        }

        public IActionResult GetUsersNoticeBox(View_UserNoticeInput input)
        {
            var res = _doorNoticeService.GetUserNotice(input);
            return ReturnJsonResult(res);
        }
    }
}
