using System.ComponentModel.DataAnnotations;

namespace ProgrammeManagementSystem.Models
{
    public class Registration
    {
        public int RegistrationID { get; set; }

        [Required]
        [Display(Name = "Student")]
        public int StudentID { get; set; }

        [Required]
        [Display(Name = "Module")]
        public int ModuleID { get; set; }

        [Display(Name = "Date Registered")]
        [DataType(DataType.Date)]
        public DateTime DateRegistered { get; set; } = DateTime.Now;

        // Navigation properties
        public Student? Student { get; set; }
        public Module? Module { get; set; }
    }
}
