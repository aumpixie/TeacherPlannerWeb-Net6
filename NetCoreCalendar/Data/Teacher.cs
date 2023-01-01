using Microsoft.AspNetCore.Identity;

namespace NetCoreCalendar.Data
{
    public class Teacher : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
