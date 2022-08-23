using AutoMapper;
using NetCoreCalendar.Data;
using NetCoreCalendar.Models;

namespace NetCoreCalendar.Configuration
{
    public class MapperConfig : Profile
    {

        public MapperConfig()
        {
            CreateMap<Student, StudentVM>().ReverseMap();
        }
    }
}
