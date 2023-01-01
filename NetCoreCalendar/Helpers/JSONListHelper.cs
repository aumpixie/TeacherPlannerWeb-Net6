using NetCoreCalendar.Data;
using NetCoreCalendar.Models;

namespace NetCoreCalendar.Helpers
{
    public static class JSONListHelper
    {
        /**
         * Helps to serialize the list of lessons into the format that we can use in the calendar
         * to show the planned lessons there
         **/
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

        /**
         * Helps to serialize the list of students into the format that we can use in the calendar
         * to show the students that are connected to the corresponding lessons there
         **/
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

    /** Two nested Data Classes that we use only for JSONListHelper;
     * All Properies begin from the lower case, as required by the calendar documentation
     **/
    public class Event
    {
        public int id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public int resourceId { get; set; }
        public string? title { get; set; }

    }

    public class Resource
    {
        public int id { get; set; }
        public string? title { get; set; }
    }
}
