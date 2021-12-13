using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.Services.IServices
{
    public interface IDoorNoticeService : IApplicationService
    {
        Base_PageOutput<List<View_DoorNoticeOutput>> GetDoorNotice(View_DoorNoticeInput input);

        DoorNotice CreateDoorNotice(DoorNotice model);
        bool UpdateDoorNotice(DoorNotice model);

        bool DeleteDoorNotice(int id);

        DoorNotice GetDoorNoticeItem(int id);

        DoorNotice GetDoorNewestNotice(int door_id);

        Base_PageOutput<List<View_UserNoticeOutput>> GetUserNotice(View_UserNoticeInput input);
    }
}
