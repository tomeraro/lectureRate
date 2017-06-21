using lecturate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lecturate.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // ------ Auto complete ------- //
        //Auto-Complete Schools input field
        public JsonResult GetSchools(string term)
        {
            ReviewDBContext db = new ReviewDBContext();
            List<string> schools;
            schools = db.Schools.Where(x => x.Name.Contains(term))
                .Select(y => y.Name).ToList();

            return Json(schools, JsonRequestBehavior.AllowGet);
        }

        //Auto-Complete Courses input field
        public JsonResult GetCourses(string term)
        {
            ReviewDBContext db = new ReviewDBContext();
            List<string> courses;
            courses = db.Courses.Where(x => x.Name.Contains(term))
                .Select(y => y.Name).ToList();

            return Json(courses, JsonRequestBehavior.AllowGet);
        }

        //Auto-Complete Lecturer input field
        public JsonResult GetLecturers(string term)
        {
            ReviewDBContext db = new ReviewDBContext();
            List<string> lecturers;
            if (!String.IsNullOrEmpty(term))
            {
                string[] all = term.Split(' ');
                string firstName = null, lastName = null;
                if (all.Length > 1)
                {
                    firstName = all[0];
                    lastName = all[1];
                    lecturers = db.Lecturers.Where(x => x.FirstName.StartsWith(firstName) && x.LastName.StartsWith(lastName))
                            .Select(y => y.FirstName + " " + y.LastName).ToList();
                    return Json(lecturers, JsonRequestBehavior.AllowGet);
                }

            }

            lecturers = db.Lecturers.Where(x => x.FirstName.StartsWith(term) || x.LastName.StartsWith(term))
                        .Select(y => y.FirstName + " " + y.LastName).ToList();
            return Json(lecturers, JsonRequestBehavior.AllowGet);


        }

       
    }
}