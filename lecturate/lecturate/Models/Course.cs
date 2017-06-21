using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace lecturate.Models
{
    public class Course
    {
        public int CourseID { get; set; } //primary key
        [Required]
        [Display(Name = "שם קורס")]
        public String Name { get; set; }

        [Display(Name = "תיאור הקורס")]
        public String Description { get; set; }

        [Required]
        [Display(Name = "רמת קושי")]
        public int Difficulty { get; set; }

        [Display(Name = "בית ספר")]
        public int? SchoolID { get; set; } //foreign key

        [Display(Name = "בית ספר")]
        public virtual School School { get; set; }

        [Display(Name = "המרצים המלמדים בקורס")]
        public virtual ICollection<Lecturer> AllLecturesTeachingTheCourse { get; set; }
        // public virtual ICollection<School> ListOfSchools { get; set; }
    }
}