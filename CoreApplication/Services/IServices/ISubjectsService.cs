using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.Services.IServices
{
    public interface ISubjectsService:IApplicationService
    {
        Base_PageOutput<List<View_SubjectsOutput>> GetSubjects(View_SubjectsInput input);


        List<View_SubjectsOutput> GetSubjectsByDoorID(int doorId);
        Subjects CreateSubject(Subjects model);
        bool UpdateSubject(Subjects model);
        Subjects GetSubjectById(int id);

        List<string> GetDoorTags(int? doorId);
    }
}
