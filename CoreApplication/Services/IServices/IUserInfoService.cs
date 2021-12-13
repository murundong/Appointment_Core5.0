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
    public interface IUserInfoService:IApplicationService
    {
        View_UinfoOutput GetUserInfo(string openid);

        View_UinfoOutput UpdateUserInfo_home(UserInfos model);
        View_UinfoOutput UpdateUserInfo_setting(UserInfos model);

        bool CheckHasUser(string openid);
        View_UinfoOutput GetUinfoByOpenid(string openid);

        bool SaveUserInfo(UserInfos model);

        View_InitialUserInfoOutput GetUserLst_Admin(string nick);

        bool AllocRole(int uid, Enum_UserRole role);

        bool SetUSerRemark(int uid,string remark);
    }
}
