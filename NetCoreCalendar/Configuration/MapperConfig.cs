using AutoMapper;
using NetCoreCalendar.Data;
using NetCoreCalendar.Models;

namespace NetCoreCalendar.Configuration
{
    public class MapperConfig : Profile
    {

        public MapperConfig()
        {
            CreateMap<Student, StudentCreateVM>().ReverseMap();
            CreateMap<Student, StudentVM>().ReverseMap();
            CreateMap<Lesson, LessonCreateVM>().ReverseMap();
            CreateMap<Lesson, LessonVM>().ReverseMap();
            CreateMap<Lesson, LessonDetailsVM>().ReverseMap();
        }
    }
}
