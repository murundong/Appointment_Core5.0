using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoreEntityFramework
{
    public class App_DbContext:DbContext
    {
        public App_DbContext(DbContextOptions<App_DbContext> options):base(options)
        {
        }


        public DbSet<CardTemplate> Set_CardTemplete { get; set; }
        public DbSet<UserInfos> Set_UserInfo { get; set; }
        public DbSet<Banners> Set_Banners { get; set; }
        public DbSet<Doors> Set_Doors { get; set; }
        public DbSet<Subjects> Set_Subjects { get; set; }
        public DbSet<Courses> Set_Courses { get; set; }
        public DbSet<DoorUsersCards> Set_DoorUsersCards { get; set; }
        public DbSet<DoorUsers> Set_DoorUsers { get; set; }
        public DbSet<DoorUsersAppoints> Set_DoorUserAppoints { get; set; }
        public DbSet<DoorUsersQueueAppoints> Set_DoorUserQueueAppoints { get; set; }
        public DbSet<DoorUsersCourseComments> SetF_DoorUsersCourseComments { get; set; }
        public DbSet<DoorNotice> Set_DoorNotice { get; set; }
        public DbSet<Notice> Set_Notice { get; set; }
        public DbSet<DoorUsersSubsMsg> Set_DoorUsersSubsMsg { get; set; }
        public DbSet<WX_TOKEN> Set_WX_TOKEN { get; set; }

        public DbSet<ApplyCurator> Set_ApplyCurator { get; set; }


        #region ViewSqlQuery
        //public DbSet<View_ServiceCourseModel> Set_View_ServiceCourseModel { get; set; }
        //public DbSet<View_PrevCardTemplateOutput> Set_View_PrevCardTemplateOutput { get; set; }
        //public DbSet<View_MyAppointWaitOutput> Set_View_MyAppointWaitOutput { get; set; }
        //public DbSet<View_MyAppointCompOutput_Detail> Set_View_MyAppointCompOutput_Detail { get; set; }
        //public DbSet<View_WinServiceCourseModel> set_View_WinServiceCourseModel { get; set; }
        //public DbSet<View_CourseShowOutput_AppointUser> set_View_CourseShowOutput_AppointUser { get; set; }
        //public DbSet<View_JudgeCourseOutput> Set_View_JudgeCourseOutput { get; set; }
        //public DbSet<View_DoorNoticeOutput> Set_View_DoorNoticeOutput { get; set; }
        //public DbSet<View_UserNoticeOutput> Set_View_UserNoticeOutput { get; set; }
        //public DbSet<View_UserCardsInfoOutput> Set_View_UserCardsInfoOutput { get; set; }
        //public DbSet<View_Appoint_UsersCardsInfo> Set_View_Appoint_UsersCardsInfo { get; set; }
        //public DbSet<View_DoorUserInfoOutput> Set_View_DoorUserInfoOutput { get; set; }
        //public DbSet<View_NoticeOutput> Set_View_NoticeOutput { get; set; }
        #endregion

    }
}
