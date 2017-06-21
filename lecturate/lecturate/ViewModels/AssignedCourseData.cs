using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lecturate.ViewModels
{
    public class AssignedCourseData
    {
        public int CourseID { get; set; }
        public string Name { get; set; }
        public bool Assigned { get; set; }
    }
}