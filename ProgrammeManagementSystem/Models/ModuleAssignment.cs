using System.ComponentModel.DataAnnotations;

namespace ProgrammeManagementSystem.Models
{
    public class ModuleAssignment
    {
        [Key]
        public int AssignmentID { get; set; }

        [Required]
        [Display(Name = "Lecturer")]
        public int LecturerID { get; set; }

        [Required]
        [Display(Name = "Module")]
        public int ModuleID { get; set; }

        // Navigation properties
        public Lecturer? Lecturer { get; set; }
        public Module? Module { get; set; }
    }
}