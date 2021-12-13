using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.Services.IServices
{
    public interface IDoorUsersQueueAppointsService : IApplicationService
    {
        bool CheckUserAlreadyQuee(int uid, int courseid);

        bool Queue(View_AppointCourseInput input);

        bool CancelQueue(int uid, int courseid);

        DoorUsersQueueAppoints GetQueueUser(int courseid);

        bool DeleteUserQueue(DoorUsersQueueAppoints entity);

        View_ServiceCourseModel GetQueenNoticDetail(int uid,int cid);
    }
}
