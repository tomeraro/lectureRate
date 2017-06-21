using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace lecturate.Models
{
    public class Review
    {
        public int ReviewID { get; set; } //primary key

        public int LecturerID { get; set; }
        public int CourseID { get; set; }
        public int SchoolID { get; set; }
        [Display(Name = "מוסד לימוד")]
        public int InstitutionID { get; set; }

        [Display(Name = "ממליץ על המרצה")]
        public bool UpVote { get; set; }
        [Display(Name = "לא ממליץ על המרצה")]
        public bool DownVote { get; set; }

        [Required]
        [Display(Name = "מוכנות המרצה לשיעור")]
        public int LecturerReadine { get; set; }
        [Required]
        [Range(1, 5)]
        [Display(Name = "דרך העברת השיעור")]
        public int LecturerTransferRate { get; set; }
        [Required]
        [Range(1, 5)]
        [Display(Name = "יחס המרצה לסטודנטים")]
        public int LecturerAttitude { get; set; }
        [Required]
        [Range(1, 5)]
        [Display(Name = "האם המרצה שולט בחומר")]
        public int LecturerKnowledge { get; set; }
        [Display(Name = "תגובה חיובית")]
        public String PositiveComment { get; set; }

        [Display(Name = "תגובה שלילית")]
        public String NegativeComment { get; set; }
        [Display(Name = "תאריך")]
        public DateTime DateOfReview { get; set; }

        [Required]
        [Display(Name = "שנת לימוד")]
        public int StudyingYear { get; set; }

        public virtual Lecturer lecturer { get; set; }
        public virtual Course course { get; set; }
        public virtual School school { get; set; }
        public virtual Institution institution { get; set; }

        [Display(Name = "ציון ממוצע")]
        public float AvgReview { get; set; }
    }


    public class ReviewDBContext : DbContext
    {

        public ReviewDBContext()
            : base("ReviewDBContext")
        {
        }

        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Institution> Institutions { get; set; }
    }
}