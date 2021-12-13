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
    public interface IDoorUsersCardsService :IApplicationService
    {
        View_InitialUserCardsInfoOutput GetUserLst_Door(int doorid,string nick);
        View_InitialUserCardsInfoOutput GetUserLst_SelfAppint(int doorid,int course_id, string nick);
        List<View_LstUserAllCardsOutput> GetUserALlCards(string openid, Enum_CardStatus cardStatus);
        List<View_UserCardsInfoOutput> GetUserDoorCards(int uid, int doorId);

        View_UserCardsInfoOutput GetUserCardsInfo(int? id);

        bool AddUserCards(DoorUsersCards model);

        bool UpdateUserCardsInfo(DoorUsersCards model);

        bool DeleteUserCards(int? id);

        List<View_Appoint_UsersCardsInfo> GetAppointDoorUserCardsInfo(int? doorId,int? uid);

        bool CheckCardsCanUse(int id);


        bool DeductionUserCards(int id);
        bool RebackUserCards(int? uid ,int? course_id);

        bool CheckCardLimitTimes(int uid, int card_id);

        int GetCardTempalteId(int id);

        void ProcessFreezeCard();
    }
}
