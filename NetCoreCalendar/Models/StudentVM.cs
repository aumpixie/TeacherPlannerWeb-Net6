using System.ComponentModel.DataAnnotations;

namespace NetCoreCalendar.Models
{
    public class StudentVM
    {
        public int Id { get; set; }

        [Display(Name= "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        public string? Goal { get; set; }

        //?
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:C}")]
        public decimal Rate { get; set; }

    }
}
