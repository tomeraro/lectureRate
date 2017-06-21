using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace lecturate.Models
{
    public class Institution
    {
        public int InstitutionID { get; set; }
        [Display(Name = "שם")]
        public string Name { get; set; }
        [Display(Name = "כתובת")]
        public string Address { get; set; }
        [Display(Name = "תיאור")]
        public string Description { get; set; }

        [Display(Name = "בתי ספר במוסד לימוד")]
        public virtual ICollection<School> Schools { get; set; }
    }
}