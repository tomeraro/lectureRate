using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using lecturate.Models;
using System.Data.Entity.Infrastructure;

namespace lecturate.Controllers
{
    public class ReviewController : Controller
    {
        private ReviewDBContext db = new ReviewDBContext();

        // ---- return the search results from the header of the page ---- //
        public ActionResult Index(string school, string course, string lecturer)
        {
            if (!string.IsNullOrEmpty(school) && !string.IsNullOrEmpty(course) && !string.IsNullOrEmpty(lecturer))
            {
                string[] lecturerName = lecturer.Split(' ');
                string firstName = lecturerName[0], lastName = lecturerName[1];
                var query = (from s in db.Reviews
                             where s.lecturer.FirstName == firstName && s.lecturer.LastName == lastName && s.school.Name == school && s.course.Name == course
                             orderby s.DateOfReview descending
                             select s);
              return View(query.ToList());
            }
            
            //if at list one string is null, open alert
            System.Windows.Forms.MessageBox.Show("לא הכנסת את כל המידע הנדרש. יש להכניס את כל הנתונים לשדות המתאימים.");
            return RedirectToAction("Index", "Home");
        }

    

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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






        // ------ calculates values for the graphs ------- //
        public ActionResult GetAvgsForGraph(int schoolID, int lecturerID, int courseID)
        {
            float avgReadiness2013 = 0, avgReadiness2014 = 0, avgReadiness2015 = 0;
            float avgKnowledge2013 = 0, avgKnowledge2014 = 0, avgKnowledge2015 = 0;
            float avgTransferRate2013 = 0, avgTransferRate2014 = 0, avgTransferRate2015 = 0;
            float avgAttitude2013 = 0, avgAttitude2014 = 0, avgAttitude2015 = 0;
            int countItems2013 = 0, countItems2014 = 0, countItems2015 = 0;

            var query = from s in db.Reviews where s.lecturer.LecturerID == lecturerID && s.school.SchoolID == schoolID && s.course.CourseID == courseID select s;            
            foreach (var item in query)
            {
                if(item.StudyingYear==2013)
                {
                    avgReadiness2013 = avgReadiness2013 + item.LecturerReadine;
                    avgKnowledge2013 = avgKnowledge2013 + item.LecturerKnowledge;
                    avgTransferRate2013 = avgTransferRate2013 + item.LecturerTransferRate;
                    avgAttitude2013 = avgAttitude2013 + item.LecturerAttitude;
                    countItems2013++; 
                }
                else if (item.StudyingYear == 2014)
                {
                    avgReadiness2014 = avgReadiness2014 + item.LecturerReadine;
                    avgKnowledge2014 = avgKnowledge2014 + item.LecturerKnowledge;
                    avgTransferRate2014 = avgTransferRate2014 + item.LecturerTransferRate;
                    avgAttitude2014 = avgAttitude2014 + item.LecturerAttitude;
                    countItems2014++;
                }
                else if (item.StudyingYear == 2015)
                {
                    avgReadiness2015 = avgReadiness2015 + item.LecturerReadine;
                    avgKnowledge2015 = avgKnowledge2015 + item.LecturerKnowledge;
                    avgTransferRate2015 = avgTransferRate2015 + item.LecturerTransferRate;
                    avgAttitude2015 = avgAttitude2015 + item.LecturerAttitude;
                    countItems2015++;
                }
            }
            
            if(countItems2013 > 0) 
            {
                avgReadiness2013 = avgReadiness2013 / countItems2013;
                avgKnowledge2013 = avgKnowledge2013 / countItems2013;
                avgAttitude2013 = avgAttitude2013 / countItems2013;
                avgTransferRate2013 = avgTransferRate2013 / countItems2013;
             }
            else
            {
                avgReadiness2013 = 0;
                avgKnowledge2013 = 0;
                avgAttitude2013 = 0;
                avgTransferRate2013 = 0;
            }

            if (countItems2014 > 0)
            {
                avgKnowledge2014 = avgKnowledge2014 / countItems2014;
                avgReadiness2014 = avgReadiness2014 / countItems2014;
                avgAttitude2014 = avgAttitude2014 / countItems2014;
                avgTransferRate2014 = avgTransferRate2014 / countItems2014;
            }
            else
            {
                avgReadiness2014 = 0;
                avgKnowledge2014 = 0;
                avgAttitude2014 = 0;
                avgTransferRate2014 = 0;
            }


            if (countItems2015 > 0)
            {
                avgReadiness2015 = avgReadiness2015 / countItems2015;
                avgKnowledge2015 = avgKnowledge2015 / countItems2015;
                avgAttitude2015 = avgAttitude2015 / countItems2015;
                avgTransferRate2015 = avgTransferRate2015 / countItems2015;
            }
            else
            {
                avgReadiness2015 = 0;
                avgKnowledge2015 = 0;
                avgAttitude2015 = 0;
                avgTransferRate2015 = 0;
            }

            List<Graph> graphs = new List<Graph>();
            Graph graph2013 = new Graph();
            graph2013.State = "2013";
            graph2013.freq = new Freq { LecturerReadine = (int)avgReadiness2013, LecturerTransferRate = (int)avgTransferRate2013, LecturerAttitude = (int)avgAttitude2013, LecturerKnowledge = (int)avgKnowledge2013  };
            graphs.Add(graph2013);

            Graph graph2014 = new Graph();
            graph2014.State = "2014";
            graph2014.freq = new Freq { LecturerReadine = (int)avgReadiness2014, LecturerTransferRate = (int)avgTransferRate2014, LecturerAttitude = (int)avgAttitude2014, LecturerKnowledge = (int)avgKnowledge2014 };
            graphs.Add(graph2014);

            Graph graph2015 = new Graph();
            graph2015.State = "2015";
            graph2015.freq = new Freq { LecturerReadine = (int)avgReadiness2015, LecturerTransferRate = (int)avgTransferRate2015, LecturerAttitude = (int)avgAttitude2015, LecturerKnowledge = (int)avgKnowledge2015 };
            graphs.Add(graph2015);
            
            ViewBag.one=countItems2013;
            ViewBag.two = countItems2014;
            ViewBag.three = countItems2015;
            return Json(graphs, JsonRequestBehavior.AllowGet);
        }


        // ----- Showing the google map ----- //
        public ActionResult GetGoogleMap(int InstitutionID)
        {
            List<String> address = new List<string>();
            var query = from s in db.Institutions where InstitutionID == s.InstitutionID select s;
            string map;
               foreach (var item in query)
               {
                   if (!String.IsNullOrEmpty(item.Address))
                   {
                       //map = item.Address;
                       //address.Add(map);
                       address.Add(item.Address);
                   }
               }

            return Json(address, JsonRequestBehavior.AllowGet);
        }

        
        
        // ------ add review by user to spesipic lecturer------- //
        public ActionResult CreateReviewForSpesipicLecturer(int schoolId, int courseId, int lecturerId)
        {
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateReviewForSpesipicLecturer([Bind(Include = "ReviewID,UpVote,DownVote,LecturerReadine,LecturerTransferRate,LecturerAttitude,LecturerKnowledge,StudyingYear,PositiveComment,NegativeComment,SchoolID, CourseID, LecturerID, InstitutionID")] Review review)
        {
            review.DateOfReview = System.DateTime.Now;
            review.AvgReview = (review.LecturerAttitude + review.LecturerKnowledge + review.LecturerReadine + review.LecturerTransferRate) / 4;
            string curseWords = "זונה,שרמוטה,הומו,אפס,כוס,זין,בולבול,מניאק,סקס,דפוק,חרא,מסריח,קוקסינל,לסבית";
            string[] curseArr = curseWords.Split(',');
            string[] negativeCommentArr = review.NegativeComment.Split(' ');
            string[] positiveCommentArr = review.PositiveComment.Split(' ');

         /*   for (int i = 0; i < curseArr.Length; i++)
            {
                if (negativeCommentArr.Contains(curseArr[i]))
                {
                    System.Windows.Forms.MessageBox.Show("הכנסת קללה לא מתאים פרוספר");
                    return RedirectToAction("CreateReview", "Managment");
                }
            }*/
            if (review.UpVote == true && review.DownVote == true)
            {
                System.Windows.Forms.MessageBox.Show("לא ניתן להמליץ וגם לא להמליץ");
                return RedirectToAction("Index", "Home");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Reviews.Add(review);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(review);


        }







        // ----------- Review Bar: School>>Course>>Lecturer ------------ //
        public ActionResult SchoolDetails(int schoolId)
        {
            var school = from s in db.Schools where s.SchoolID==schoolId select s;
            return View(school.ToList());
        }

        public ActionResult CourseDetails(int courseId)
        {
            var course = from s in db.Courses where s.CourseID == courseId select s;
            return View(course.ToList());
        }

        public ActionResult LecturerDetails(int lecturerId)
        {
            var lecturer = from s in db.Lecturers where s.LecturerID == lecturerId select s;
            return View(lecturer.ToList());
        }

        public ActionResult InstitutionDetails(int InstitutionID)
        {
            var institution = from s in db.Institutions where s.InstitutionID == InstitutionID select s;
            return View(institution.ToList());
        }
       
    }

    
}


