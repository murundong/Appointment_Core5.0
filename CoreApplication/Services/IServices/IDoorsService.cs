using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.Services.IServices
{
    public interface IDoorsService:IApplicationService
    {
        Base_PageOutput< List<View_TearcherDoorOutput>> GetDoors(View_DoorInput input);

        Base_PageOutput<List<View_TearcherDoorOutput>> GetTeacherDoors(View_TeacherDoorInput input);
        Base_PageOutput<List<View_TearcherDoorOutput>> GetAdminAllDoors(Base_PageInput input);
        Doors CreateDoors(Doors model);

        bool UpdateDoors(Doors model);

        Doors GetDoorsById(int id);
        View_LessonDoorInfoOutput GetDoorInfo(int doorid);
    }
}
