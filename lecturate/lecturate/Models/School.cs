using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace lecturate.Models
{
    public class School
    {
        public int SchoolID { get; set; } //primary key

        [Required]
        [Display(Name = "שם בית ספר")]
        public String Name { get; set; }

        [Display(Name = "שנת הקמה")]
        [Range(1900, 2015)]
        public int Year { get; set; }

        [Display(Name = "תיאור")]
        public String Description { get; set; }

        [Display(Name = "טלפון")]
        public String Phone { get; set; }

        [Display(Name = "בית ספר")]
        public int? InstitutionID { get; set; } //foreign key

        public virtual Institution Institution { get; set; }

        [Display(Name = "מרצים המלמדים בבית ספר")]
        public virtual ICollection<Lecturer> AllLecturesTeachingInThisSchool { get; set; }

        [Display(Name = "קורסים בבית ספר")]
        public virtual ICollection<Course> CoursesLearnedInSchool { get; set; }
    }
}