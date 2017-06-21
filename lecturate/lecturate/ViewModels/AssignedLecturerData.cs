using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lecturate.ViewModels
{
    public class AssignedLecturerData
    {
        public int LecturerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Assigned { get; set; }
    }
}