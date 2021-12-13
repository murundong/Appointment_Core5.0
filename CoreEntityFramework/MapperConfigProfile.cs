using AutoMapper;
using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework
{
    public class MapperConfigProfile : Profile
    {

        Func<DateTime?, string> ConvertTimeToString => s=>
        {
            if (s.HasValue) return Convert.ToDateTime(s).ToString("yyyy-MM-dd");
            return null;
        };
        public MapperConfigProfile()
        {
            CreateMap<Doors, View_TearcherDoorOutput>();
            CreateMap<CardTemplate, View_CardTemplateOutputItem>();
            CreateMap<CardTemplate,View_CardTemplateOutput>();
            CreateMap<UserInfos, View_UinfoOutput>()
                .ForMember(des => des.birthday, opt => opt.MapFrom(src => ConvertTimeToString(src.birthday)));
          
            CreateMap<Banners, View_BannerOutput>();
            CreateMap<Doors, View_DoorsOutput>();

            CreateMap<Subjects, View_SubjectsOutput>();


            CreateMap<CardTemplate, CardTemplate>();
            CreateMap<Subjects, Subjects>();
            CreateMap<Courses, Courses>();
            CreateMap<Courses, View_CoursesOutput>();
            CreateMap<Courses, View_CourseShowOutput>();
            CreateMap<CardTemplate, ViewDoorCardsSelect>();
        }
    }
}
