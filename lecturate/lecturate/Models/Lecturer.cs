using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace lecturate.Models
{
    public class Lecturer
    {
        public int LecturerID { get; set; } //primary key

        //[Required(ErrorMessage = "First Name is required")]
        [Required]
        [Display(Name = "שם פרטי")]
        public String FirstName { get; set; }
        [Required]
        [Display(Name = "שם משפחה")]
        public String LastName { get; set; }

        [Display(Name = "שם המרצה")]
        public String FullName
        {
            get { return FirstName + " " + LastName; }
        }

        [Range(1, 50)]
        [Display(Name = "וותק")]
        public int Seniority { get; set; }
        [Range(1, 10)]
        [Display(Name = "ציון כללי")]
        public float GeneralGradeOfLecturer { get; set; }

        [Display(Name = "הקורסים שהמרצה מלמד")]
        public virtual ICollection<Course> AllCoursesTeachedByLecturer { get; set; }

        [Display(Name = "שייכות לבית ספר")]
        public virtual ICollection<School> AllSchoolLecturerBelongsTo { get; set; }
    }
}