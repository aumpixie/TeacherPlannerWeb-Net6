using NetCoreCalendar.Data;
using NetCoreCalendar.Models;

namespace NetCoreCalendar.Helpers
{
    public static class JSONListHelper
    {
        public static string GetEventListJSONString(List<LessonVM> lessons)
        {
            var eventList = new List<Event>();
            foreach(var lesson in lessons)
            {
                var myEvent = new Event()
                {
                    id = lesson.Id,
                    start = lesson.Start,
                    end = lesson.End,
                    resourceId = lesson.Student.Id,
                    title = lesson.Student.FirstName + " " + lesson.Student.LastName
                };
                eventList.Add(myEvent);
            }
            return System.Text.Json.JsonSerializer.Serialize(eventList);
        }

        public static string GetResourceListJSONString(List<StudentVM> students)
        {
            var resourceList = new List<Resource>();
            foreach (var student in students)
            {
                var resource = new Resource()
                {
                    id = student.Id,
                    title = student.FirstName + " " + student.LastName 
                };
                resourceList.Add(resource);
            }
            return System.Text.Json.JsonSerializer.Serialize(resourceList);

        }

    }


    public class Event
    {
        public int id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public int resourceId { get; set; }
        public string title { get; set; }

    }

    public class Resource
    {
        public int id { get; set; }
        public string title { get; set; }
    }
}
