using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.Services.IServices
{
    public interface IDoorUsersCourseCommentsService:IApplicationService
    {
        bool AddJudge(DoorUsersCourseComments input);
    }
}
