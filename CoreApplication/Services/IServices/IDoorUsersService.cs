using CoreEntityFramework.Enum;
using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.Services.IServices
{
    public interface IDoorUsersService:IApplicationService
    {
        void AddUserAttention(string openid, int doorid);

        bool CheckUserBlackList(string openid,int doorid);
        bool SetUserRemark(int id, string remark);
        bool AllocRole(int id, Enum_UserRole role);

        View_DoorUserInfoOutput GetDoorUserInfo(int id);

        bool CheckHasAdminMenu(int uid);

        DoorUsers GetDoorUsersByUID(int door_id, int uid);
    }
}
