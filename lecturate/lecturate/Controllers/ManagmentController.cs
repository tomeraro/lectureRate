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
using lecturate.ViewModels;

namespace lecturate.Controllers
{

    
    public class ManagmentController : Controller
    {
        private ReviewDBContext db = new ReviewDBContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Manager()
        {
            return View();
        }


        //-------------Lecturer-------------

        // GET: /Managment/
        [Authorize(Roles = "Admin")]
        public ActionResult IndexLecturer(string sortOrder, string searchString)
        {
            ViewBag.FirstNameSortParm = sortOrder == "First Name" ? "FirstName_desc" : "First Name";
            ViewBag.LastNameSortParm = sortOrder == "Last Name" ? "LastName_desc" : "Last Name";
            ViewBag.SenioritySortParm = sortOrder == "Seniority" ? "Seniority_desc" : "Seniority";
            ViewBag.GradeSortParm = sortOrder == "Grade" ? "Grade_desc" : "Grade";

            var lecturer = from s in db.Lecturers
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                string[] words = searchString.Split(' ');
                string FirstNameSplit = null, LastNameSplit = null;
                if (words.Length > 1)
                {
                    FirstNameSplit = words[0];
                    LastNameSplit = words[1];
                }


                if (words.Length == 1)
                {
                    lecturer = lecturer.Where(s => s.FirstName.Contains(searchString) || s.LastName.Contains(searchString));
                }

                else if (words.Length == 2)
                {
                    lecturer = lecturer.Where(s => s.FirstName.Equals(FirstNameSplit) && s.LastName.Equals(LastNameSplit));
                }
            }

            switch (sortOrder)
            {
                case "First Name":
                    lecturer = lecturer.OrderBy(s => s.FirstName);
                    break;
                case "FirstName_desc":
                    lecturer = lecturer.OrderByDescending(s => s.FirstName);
                    break;
                case "Last Name":
                    lecturer = lecturer.OrderBy(s => s.LastName);
                    break;
                case "LastName_desc":
                    lecturer = lecturer.OrderByDescending(s => s.LastName);
                    break;

                case "Seniority":
                    lecturer = lecturer.OrderBy(s => s.Seniority);
                    break;

                case "Seniority_desc":
                    lecturer = lecturer.OrderByDescending(s => s.Seniority);
                    break;

                case "Grade":
                    lecturer = lecturer.OrderBy(s => s.GeneralGradeOfLecturer);
                    break;

                case "Grade_desc":
                    lecturer = lecturer.OrderByDescending(s => s.GeneralGradeOfLecturer);
                    break;

                default:
                    lecturer = lecturer.OrderBy(s => s.FirstName);
                    break;
            }
            return View(lecturer.ToList());
        }

        // GET: /Managment/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult DetailsLecturer(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lecturer lecturer = db.Lecturers.Find(id);
            if (lecturer == null)
            {
                return HttpNotFound();
            }
            return View(lecturer);
        }

        // GET: /Managment/Create
        [Authorize(Roles = "Admin")]
        public ActionResult CreateLecturer()
        {
            var lecturer = new Lecturer();
            lecturer.AllSchoolLecturerBelongsTo = new List<School>();
            PopulateAssignedSchoolData(lecturer);
            return View();
        }

        // POST: /Managment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateLecturer([Bind(Include = "LecturerID,FirstName,LastName,Seniority,GeneralGradeOfLecturer")] Lecturer lecturer, string[] selectedSchools)
        {
            if (selectedSchools != null)
            {
                lecturer.AllSchoolLecturerBelongsTo = new List<School>();
                foreach (var school in selectedSchools)
                {
                    var schoolToAdd = db.Schools.Find(int.Parse(school));
                    lecturer.AllSchoolLecturerBelongsTo.Add(schoolToAdd);
                }
            }


            if (ModelState.IsValid)
            {
                db.Lecturers.Add(lecturer);
                db.SaveChanges();
                return RedirectToAction("IndexLecturer");
            }
            PopulateAssignedSchoolData(lecturer);
            return View(lecturer);
        }

        // GET: /Managment/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult EditLecturer(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lecturer lecturer = db.Lecturers.Include(i => i.AllSchoolLecturerBelongsTo).Where(i => i.LecturerID == id).Single();
            PopulateAssignedSchoolData(lecturer);
            if (lecturer == null)
            {
                return HttpNotFound();
            }
            return View(lecturer);
        }

        [Authorize(Roles = "Admin")]
        private void PopulateAssignedSchoolData(Lecturer lecturer)
        {
            var allSchools = db.Schools;
            var lecturerSchool = new HashSet<int>(lecturer.AllSchoolLecturerBelongsTo.Select(c => c.SchoolID));
            var viewModel = new List<AssignedSchoolData>();
            foreach (var school in allSchools)
            {
                viewModel.Add(new AssignedSchoolData
                {
                    SchoolID = school.SchoolID,
                    Name = school.Name,
                    Assigned = lecturerSchool.Contains(school.SchoolID)
                });
            }
            ViewBag.Schools = viewModel;
        }

        // POST: /Managment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        /*  public ActionResult EditLecturer([Bind(Include = "LecturerID,FirstName,LastName,Seniority,GeneralGradeOfLecturer")] Lecturer lecturer)
          {
              if (ModelState.IsValid)
              {
                  db.Entry(lecturer).State = EntityState.Modified;
                  db.SaveChanges();
                  return RedirectToAction("IndexLecturer");
              }
              return View(lecturer);
          }
        */
        [Authorize(Roles = "Admin")]
        public ActionResult EditLecturer(int? id, string[] selectedSchools)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var lecturerToUpdate = db.Lecturers
               .Include(i => i.AllSchoolLecturerBelongsTo)
               .Where(i => i.LecturerID == id)
               .Single();

            if (TryUpdateModel(lecturerToUpdate, "",
               new string[] { "LecturerID", "FirstName", "LastName", "Seniority", "GeneralGradeOfLecturer" }))
            {
                try
                {
                    UpdateLectuerSchool(selectedSchools, lecturerToUpdate);

                    db.SaveChanges();

                    return RedirectToAction("IndexLecturer");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedSchoolData(lecturerToUpdate);
            return View(lecturerToUpdate);
        }

        [Authorize(Roles = "Admin")]
        private void UpdateLectuerSchool(string[] selectedSchools, Lecturer lecturerToUpdate)
        {
            if (selectedSchools == null)
            {
                lecturerToUpdate.AllSchoolLecturerBelongsTo = new List<School>();
                return;
            }

            var selectedSchoolsHS = new HashSet<string>(selectedSchools);
            var lecturerSchools = new HashSet<int>
                (lecturerToUpdate.AllSchoolLecturerBelongsTo.Select(c => c.SchoolID));
            foreach (var school in db.Schools)
            {
                if (selectedSchoolsHS.Contains(school.SchoolID.ToString()))
                {
                    if (!lecturerSchools.Contains(school.SchoolID))
                    {
                        lecturerToUpdate.AllSchoolLecturerBelongsTo.Add(school);
                    }
                }
                else
                {
                    if (lecturerSchools.Contains(school.SchoolID))
                    {
                        lecturerToUpdate.AllSchoolLecturerBelongsTo.Remove(school);
                    }
                }
            }
        }



        // GET: /Managment/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteLecturer(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lecturer lecturer = db.Lecturers.Find(id);
            if (lecturer == null)
            {
                return HttpNotFound();
            }
            return View(lecturer);
        }

        // POST: /Managment/Delete/5
        [HttpPost, ActionName("DeleteLecturer")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmedLecturer(int id)
        {
            Lecturer lecturer = db.Lecturers.Where(i => i.LecturerID == id).Single();
            db.Lecturers.Remove(lecturer);


            /* TODO need to delete from AllLecturesTeachingTheCourse in "Course" the specific lecturer!!
             
            var lecturers = db.Lecturers.Include(i => i.AllCoursesTeachedByLecturer).Where(d => d.LecturerID == id);

            //delete all courses that are learned in this school 
            foreach (var lecturer1 in lecturers)
            {
                db.Courses.Remove(lecturer1);
            }
            
            */
            db.SaveChanges();
            return RedirectToAction("IndexLecturer");
        }


        //the same function for all (lecturer,school,course)
        [Authorize(Roles = "Admin")]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        //-----------------School-----------------

        // GET: /School/
        [Authorize(Roles = "Admin")]
        public ActionResult IndexSchool(string sortOrder, string searchString)
        {
            ViewBag.SchoolNameSortParm = sortOrder == "School Name" ? "SchoolName_desc" : "School Name";
            ViewBag.SchoolIDSortParm = sortOrder == "School ID" ? "SchoolID_desc" : "School ID";

            var school = from s in db.Schools
                         select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                school = school.Where(s => s.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "School Name":
                    school = school.OrderBy(s => s.Name);
                    break;
                case "SchoolName_desc":
                    school = school.OrderByDescending(s => s.Name);
                    break;

                default:
                    school = school.OrderBy(s => s.Name);
                    break;
            }
            return View(school.ToList());
        }

        // GET: /School/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult DetailsSchool(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            School school = db.Schools.Find(id);
            if (school == null)
            {
                return HttpNotFound();
            }
            return View(school);
        }

        // GET: /School/Create
        [Authorize(Roles = "Admin")]
        public ActionResult CreateSchool()
        {
            var school = new School();
            school.CoursesLearnedInSchool = new List<Course>();
            PopulateAssignedCourseData(school);
            return View();
        }

        // POST: /School/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateSchool([Bind(Include = "SchoolID,Name,Phone,Year,Description")] School school, string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                school.CoursesLearnedInSchool = new List<Course>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = db.Courses.Find(int.Parse(course));
                    school.CoursesLearnedInSchool.Add(courseToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                db.Schools.Add(school);
                db.SaveChanges();
                return RedirectToAction("IndexSchool");
            }

            PopulateAssignedCourseData(school);
            return View(school);
        }

        // GET: /School/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult EditSchool(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // School school = db.Schools.Find(id);
            School school = db.Schools.Include(i => i.CoursesLearnedInSchool).Where(i => i.SchoolID == id).Single();
            PopulateAssignedCourseData(school);
            if (school == null)
            {
                return HttpNotFound();
            }
            return View(school);
        }

        [Authorize(Roles = "Admin")]
        private void PopulateAssignedCourseData(School school)
        {
            var allCourses = db.Courses;
            var schoolCourses = new HashSet<int>(school.CoursesLearnedInSchool.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Name = course.Name,
                    Assigned = schoolCourses.Contains(course.CourseID)
                });
            }
            ViewBag.Courses = viewModel;
        }

        // POST: /School/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult EditSchool(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var SchoolToUpdate = db.Schools
               .Include(i => i.CoursesLearnedInSchool)
               .Where(i => i.SchoolID == id)
               .Single();

            if (TryUpdateModel(SchoolToUpdate, "",
               new string[] { "Name","Phone","Year","Description" }))
            {
                try
                {

                    UpdateSchoolCourses(selectedCourses, SchoolToUpdate);

                    db.SaveChanges();

                    return RedirectToAction("IndexSchool");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedCourseData(SchoolToUpdate);
            return View(SchoolToUpdate);
        }

        [Authorize(Roles = "Admin")]
        private void UpdateSchoolCourses(string[] selectedCourses, School SchoolToUpdate)
        {
            if (selectedCourses == null)
            {
                SchoolToUpdate.CoursesLearnedInSchool = new List<Course>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var schoolCourses = new HashSet<int>
                (SchoolToUpdate.CoursesLearnedInSchool.Select(c => c.CourseID));
            foreach (var course in db.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!schoolCourses.Contains(course.CourseID))
                    {
                        SchoolToUpdate.CoursesLearnedInSchool.Add(course);
                    }
                }
                else
                {
                    if (schoolCourses.Contains(course.CourseID))
                    {
                        SchoolToUpdate.CoursesLearnedInSchool.Remove(course);
                    }
                }
            }
        }



        // GET: /School/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteSchool(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            School school = db.Schools.Find(id);
            if (school == null)
            {
                return HttpNotFound();
            }
            return View(school);
        }

        // POST: /School/Delete/5
        [HttpPost, ActionName("DeleteSchool")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmedSchool(int id)
        {
            School school = db.Schools.Where(i => i.SchoolID == id).Single();

            db.Schools.Remove(school);

            var courses = db.Courses.Where(d => d.SchoolID == id);

            //delete all courses that are learned in this school 
            foreach (var course in courses)
            {
                db.Courses.Remove(course);
            }

            db.SaveChanges();
            return RedirectToAction("IndexSchool");
        }

        //-------------Course------------

        // GET: /Course/
        [Authorize(Roles = "Admin")]
        public ActionResult IndexCourse(string sortOrder, string searchString)
        {
            ViewBag.CourseNameSortParm = sortOrder == "Course Name" ? "CourseName_desc" : "Course Name";
            ViewBag.DifficultySortParm = sortOrder == "Difficulty" ? "Difficulty_desc" : "Difficulty";
            ViewBag.SchoolSortParm = sortOrder == "School" ? "School_desc" : "School";


            var course = from c in db.Courses
                         select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                course = course.Where(c => c.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Course Name":
                    course = course.OrderBy(c => c.Name);
                    break;
                case "CourseName_desc":
                    course = course.OrderByDescending(c => c.Name);
                    break;

                case "Difficulty":
                    course = course.OrderBy(c => c.Difficulty);
                    break;

                case "Difficulty_desc":
                    course = course.OrderByDescending(c => c.Difficulty);
                    break;

                case "School":
                    course = course.OrderBy(c => c.SchoolID);
                    break;
                case "School_desc":
                    course = course.OrderByDescending(c => c.SchoolID);
                    break;

            }
            return View(course.ToList());
        }


        // GET: /Course/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult DetailsCourse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: /Course/Create
        [Authorize(Roles = "Admin")]
        public ActionResult CreateCourse()
        {
            var course = new Course();
            course.AllLecturesTeachingTheCourse = new List<Lecturer>();
            PopulateDepartmentsDropDownList();
            PopulateAssignedLecturerData(course);
            return View();
        }

        // POST: /Course/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateCourse([Bind(Include = "CourseID,Name,Description,Difficulty,SchoolID")] Course course, string[] selectedLecturers)
        {
            if (selectedLecturers != null)
            {
                course.AllLecturesTeachingTheCourse = new List<Lecturer>();
                foreach (var lecturer in selectedLecturers)
                {
                    var lecturerToAdd = db.Lecturers.Find(int.Parse(lecturer));
                    course.AllLecturesTeachingTheCourse.Add(lecturerToAdd);
                }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    db.Courses.Add(course);
                    db.SaveChanges();
                    return RedirectToAction("IndexCourse");
                }
            }

            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateDepartmentsDropDownList(course.SchoolID);
            PopulateAssignedLecturerData(course);

            return View(course);
        }


        [Authorize(Roles = "Admin")]
        private void PopulateAssignedLecturerData(Course course)
        {
            var allLecturers = db.Lecturers;
            var courseLecturers = new HashSet<int>(course.AllLecturesTeachingTheCourse.Select(c => c.LecturerID));
            var viewModel = new List<AssignedLecturerData>();
            foreach (var lecturer in allLecturers)
            {
                viewModel.Add(new AssignedLecturerData
                {
                    LecturerID = lecturer.LecturerID,
                    FirstName = lecturer.FirstName,
                    LastName = lecturer.LastName,
                    Assigned = courseLecturers.Contains(lecturer.LecturerID)
                });
            }
            ViewBag.Lecturers = viewModel;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditCourse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Include(i => i.AllLecturesTeachingTheCourse).Where(i => i.CourseID == id).Single();
            PopulateAssignedLecturerData(course);
            PopulateDepartmentsDropDownList(course.SchoolID);
            if (course == null)
            {
                return HttpNotFound();
            }

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult EditCourse(int? id, string[] selectedLecturers)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var courseToUpdate = db.Courses.Include(i => i.AllLecturesTeachingTheCourse).Where(i => i.CourseID == id).Single();
            if (TryUpdateModel(courseToUpdate, "",
               new string[] { "Name", "Description", "Difficulty", "SchoolID", "AllLecturesTeachingTheCourse" }))
            {
                try
                {
                    UpdateCourseLecturers(selectedLecturers, courseToUpdate);
                    db.SaveChanges();

                    return RedirectToAction("IndexCourse");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateDepartmentsDropDownList(courseToUpdate.SchoolID);
            PopulateAssignedLecturerData(courseToUpdate);
            return View(courseToUpdate);
        }

        [Authorize(Roles = "Admin")]
        private void UpdateCourseLecturers(string[] selectedLecturers, Course CourseToUpdate)
        {
            if (selectedLecturers == null)
            {
                CourseToUpdate.AllLecturesTeachingTheCourse = new List<Lecturer>();
                return;
            }

            var selectedLecturersHS = new HashSet<string>(selectedLecturers);
            var CourseLecturers = new HashSet<int>
                (CourseToUpdate.AllLecturesTeachingTheCourse.Select(c => c.LecturerID));
            foreach (var lecturer in db.Lecturers)
            {
                if (selectedLecturersHS.Contains(lecturer.LecturerID.ToString()))
                {
                    if (!CourseLecturers.Contains(lecturer.LecturerID))
                    {
                        CourseToUpdate.AllLecturesTeachingTheCourse.Add(lecturer);
                    }
                }
                else
                {
                    if (CourseLecturers.Contains(lecturer.LecturerID))
                    {
                        CourseToUpdate.AllLecturesTeachingTheCourse.Remove(lecturer);
                    }
                }
            }
        }



        [Authorize(Roles = "Admin")]
        private void PopulateDepartmentsDropDownList(object selectedSchool = null)
        {
            var schoolsQuery = from d in db.Schools
                               orderby d.Name
                               select d;
            ViewBag.SchoolID = new SelectList(schoolsQuery, "SchoolID", "Name", selectedSchool);
        }





        // GET: /Course/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteCourse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: /Course/Delete/5
        [HttpPost, ActionName("DeleteCourse")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmedCourse(int id)
        {
            //Course course = db.Courses.Find(id);
            Course course = db.Courses.Include(i => i.AllLecturesTeachingTheCourse).Where(i => i.CourseID == id).Single();
            db.Courses.Remove(course);


            db.SaveChanges();
            return RedirectToAction("IndexCourse");
        }








//------------- Auto complete----------


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



        //---------------Reviews-----------------
        [Authorize(Roles = "Admin")]
        public ActionResult AllReviews(string sortOrder, string searchStringLecturer, string searchStringCourse, string searchStringSchool)
        {
            string FirstNameSplit = null, LastNameSplit = null;
            string[] words = null;

            ViewBag.FirstNameSortParm = sortOrder == "First Name" ? "FirstName_desc" : "First Name";
            ViewBag.SchoolNameSortParm = sortOrder == "School Name" ? "SchoolName_desc" : "School Name";
            ViewBag.CourseNameSortParm = sortOrder == "Course Name" ? "CourseName_desc" : "Course Name";

            var review = from r in db.Reviews
                         select r;

            var queryReviewsByYear = (from c in db.Reviews
                                      group c by c.StudyingYear into d
                                      select new
                                      {
                                          שנה = d.Key,
                                          כמות = d.Count()
                                      });


            ViewBag.number = queryReviewsByYear;

            switch (sortOrder)
            {
                case "First Name":
                    review = review.OrderBy(r => r.lecturer.FirstName);
                    break;
                case "FirstName_desc":
                    review = review.OrderByDescending(r => r.lecturer.FirstName);
                    break;
                case "School Name":
                    review = review.OrderBy(r => r.school.Name);
                    break;
                case "SchoolName_desc":
                    review = review.OrderByDescending(r => r.school.Name);
                    break;
                case "Course Name":
                    review = review.OrderBy(r => r.course.Name);
                    break;
                case "CourseName_desc":
                    review = review.OrderByDescending(r => r.course.Name);
                    break;

                default:
                    review = review.OrderBy(r => r.lecturer.FirstName);
                    break;
            }

            if (searchStringLecturer != null)
            {
                words = searchStringLecturer.Split(' ');
            }

            if (!String.IsNullOrEmpty(searchStringLecturer) && String.IsNullOrEmpty(searchStringCourse) && String.IsNullOrEmpty(searchStringSchool)) //lecturer isn't null, school and course are null
            {

                if (words.Length == 1)
                {
                    review = review.Where(r => r.lecturer.FirstName.Contains(searchStringLecturer) || r.lecturer.LastName.Contains(searchStringLecturer));
                }

                else if (words.Length == 2)
                {
                    FirstNameSplit = words[0];
                    LastNameSplit = words[1];

                    review = review.Where(r => r.lecturer.FirstName.Contains(FirstNameSplit) && r.lecturer.LastName.Contains(LastNameSplit));
                }

            }
            else if (!String.IsNullOrEmpty(searchStringCourse) && !String.IsNullOrEmpty(searchStringLecturer) && String.IsNullOrEmpty(searchStringSchool)) //lecturer and course are not null, school is null
            {
                if (words.Length == 1)
                {
                    if (review.Where(r => r.lecturer.FirstName.Contains(searchStringLecturer)) != null)
                    {
                        FirstNameSplit = searchStringLecturer;
                    }

                    else if (review.Where(r => r.lecturer.LastName.Contains(searchStringLecturer)) != null)
                    {
                        LastNameSplit = searchStringLecturer;
                    }

                    if (FirstNameSplit != null)
                    {
                        review = review.Where(r => r.lecturer.FirstName.Contains(FirstNameSplit) && r.course.Name.Contains(searchStringCourse));
                    }

                    else if (LastNameSplit != null)
                    {
                        review = review.Where(r => r.lecturer.LastName.Contains(LastNameSplit) && r.course.Name.Contains(searchStringCourse));
                    }
                }

                else if (words.Length == 2)
                {
                    FirstNameSplit = words[0];
                    LastNameSplit = words[1];

                    review = review.Where(r => r.lecturer.FirstName.Contains(FirstNameSplit) && r.lecturer.LastName.Contains(LastNameSplit) && r.course.Name.Contains(searchStringCourse));

                }
            }

            else if (String.IsNullOrEmpty(searchStringLecturer) && !String.IsNullOrEmpty(searchStringCourse) && String.IsNullOrEmpty(searchStringSchool)) //course isn't null, lecturer and school are null
            {
                review = review.Where(r => r.course.Name.Contains(searchStringCourse));
            }

            else if (!String.IsNullOrEmpty(searchStringSchool) && String.IsNullOrEmpty(searchStringCourse) && String.IsNullOrEmpty(searchStringLecturer)) //school isn't null, lecturer and course are null
            {
                review = review.Where(r => r.school.Name.Contains(searchStringSchool));
            }

            else if (!String.IsNullOrEmpty(searchStringSchool) && !String.IsNullOrEmpty(searchStringLecturer) && String.IsNullOrEmpty(searchStringCourse)) //School & Lecturer aren't empty, course is empty
            {
                if (words.Length == 1)
                {
                    if (review.Where(r => r.lecturer.FirstName.Contains(searchStringLecturer)) != null)
                    {
                        FirstNameSplit = searchStringLecturer;
                    }

                    else if (review.Where(r => r.lecturer.LastName.Contains(searchStringLecturer)) != null)
                    {
                        LastNameSplit = searchStringLecturer;
                    }

                    if (FirstNameSplit != null)
                    {
                        review = review.Where(r => r.school.Name.Contains(searchStringSchool) && r.lecturer.FirstName.Contains(searchStringLecturer));
                    }

                    else if (LastNameSplit != null)
                    {
                        review = review.Where(r => r.school.Name.Contains(searchStringSchool) && r.lecturer.LastName.Contains(searchStringLecturer));
                    }
                }

                else if (words.Length == 2)
                {
                    FirstNameSplit = words[0];
                    LastNameSplit = words[1];

                    review = review.Where(r => r.school.Name.Contains(searchStringSchool) && r.lecturer.FirstName.Contains(FirstNameSplit) && r.lecturer.LastName.Contains(LastNameSplit));
                }
            }

            else if (!String.IsNullOrEmpty(searchStringSchool) && String.IsNullOrEmpty(searchStringLecturer) && !String.IsNullOrEmpty(searchStringCourse)) //School & Course aren't empty, lecturer is empty
            {
                review = review.Where(r => r.school.Name.Contains(searchStringSchool) && r.course.Name.Contains(searchStringCourse));
            }

            else if (!String.IsNullOrEmpty(searchStringLecturer) && !String.IsNullOrEmpty(searchStringCourse) && !String.IsNullOrEmpty(searchStringSchool))
            {
                if (words.Length == 1)
                {
                    if (review.Where(r => r.lecturer.FirstName.Contains(searchStringLecturer)) != null)
                    {
                        FirstNameSplit = searchStringLecturer;
                    }

                    else if (review.Where(r => r.lecturer.LastName.Contains(searchStringLecturer)) != null)
                    {
                        LastNameSplit = searchStringLecturer;
                    }

                    if (FirstNameSplit != null)
                    {
                        review = review.Where(r => r.lecturer.FirstName.Contains(FirstNameSplit) && r.course.Name.Contains(searchStringCourse) && r.school.Name.Contains(searchStringSchool));
                    }

                    else if (LastNameSplit != null)
                    {
                        review = review.Where(r => r.lecturer.LastName.Contains(LastNameSplit) && r.course.Name.Contains(searchStringCourse) && r.school.Name.Contains(searchStringSchool));
                    }
                }

                else if (words.Length == 2)
                {
                    FirstNameSplit = words[0];
                    LastNameSplit = words[1];

                    review = review.Where(r => r.lecturer.FirstName.Contains(FirstNameSplit) && r.lecturer.LastName.Contains(LastNameSplit) && r.course.Name.Contains(searchStringCourse) && r.school.Name.Contains(searchStringSchool));
                }
            }

            return View(review.ToList());
        }


        // GET: /Review/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult DetailsReview(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // GET: /Review/Create
        public ActionResult CreateReview()
        {
            PopulateCourseDropDownList();
            PopulateLecturerDropDownList();
            PopulateSchoolDropDownList();
            PopulateInstitutionDropDownList();
            return View();
        }

        // POST: /Review/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateReview([Bind(Include = "ReviewID,UpVote,DownVote,LecturerReadine,LecturerTransferRate,LecturerAttitude,LecturerKnowledge,StudyingYear,PositiveComment,NegativeComment,SchoolID, CourseID, LecturerID, InstitutionID")] Review review)
        {
            review.DateOfReview = System.DateTime.Now;
            review.AvgReview = (review.LecturerAttitude + review.LecturerKnowledge + review.LecturerReadine + review.LecturerTransferRate) / 4;
            string curseWords = "זונה,שרמוטה,הומו,אפס,כוס,זין,בולבול,מניאק,סקס,דפוק,חרא,מסריח,קוקסינל,לסבית";
            string[] curseArr = curseWords.Split(',');
            string[] negativeCommentArr = review.NegativeComment.Split(' ');
            string[] positiveCommentArr = review.PositiveComment.Split(' ');

            for (int i = 0; i < curseArr.Length; i++)
            {
                if (negativeCommentArr.Contains(curseArr[i]))
                {
                    System.Windows.Forms.MessageBox.Show("הכנסת קללה לא מתאים פרוספר");
                    return RedirectToAction("CreateReview");
                }
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

            PopulateCourseDropDownList(review.CourseID);
            PopulateLecturerDropDownList(review.LecturerID);
            PopulateSchoolDropDownList(review.SchoolID);
            PopulateInstitutionDropDownList(review.InstitutionID);

            return View(review);


        }

        public void PopulateCourseDropDownList(object selectedCourse = null)
        {
            var coursesQuery = from d in db.Courses
                               orderby d.Name
                               select d;
            ViewBag.CourseID = new SelectList(coursesQuery, "CourseID", "Name", selectedCourse);
        }


        public void PopulateLecturerDropDownList(object selectedLecturer = null)
        {
            var lecturersQuery = from d in db.Lecturers
                                 orderby d.FirstName
                                 select d;
            ViewBag.LecturerID = new SelectList(lecturersQuery, "LecturerID", "FullName", selectedLecturer);
        }

        public void PopulateSchoolDropDownList(object selectedSchool = null)
        {
            var schoolsQuery = from d in db.Schools
                               orderby d.Name
                               select d;
            ViewBag.SchoolID = new SelectList(schoolsQuery, "SchoolID", "Name", selectedSchool);
        }

        public void PopulateInstitutionDropDownList(object selectedInstitution = null)
        {
            var institutionsQuery = from d in db.Institutions
                                    orderby d.Name
                                    select d;
            ViewBag.InstitutionID = new SelectList(institutionsQuery, "InstitutionID", "Name", selectedInstitution);
        }



        // GET: /Review/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult EditReview(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            PopulateCourseDropDownList(review.CourseID);
            PopulateLecturerDropDownList(review.LecturerID);
            PopulateSchoolDropDownList(review.SchoolID);
            PopulateInstitutionDropDownList(review.InstitutionID);
            return View(review);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult EditReview(int? id, string[] selectedLecturers = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var reviewToUpdate = db.Reviews.Find(id);
            reviewToUpdate.AvgReview = (reviewToUpdate.LecturerAttitude + reviewToUpdate.LecturerKnowledge + reviewToUpdate.LecturerReadine + reviewToUpdate.LecturerTransferRate) / 4;

            if (TryUpdateModel(reviewToUpdate, "",
               new string[] { "ReviewID", "lecturer", "SchoolID", "CourseID", "LecturerID", "UpVote", "DownVote", "LecturerReadine", "LecturerTransferRate", "LecturerAttitude", "LecturerKnowledge", "StudyingYear", "PositiveComment", "NegativeComment", "AvgReview", "InstitutionID" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("AllReviews");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateCourseDropDownList(reviewToUpdate.CourseID);
            PopulateLecturerDropDownList(reviewToUpdate.LecturerID);
            PopulateSchoolDropDownList(reviewToUpdate.SchoolID);
            PopulateInstitutionDropDownList(reviewToUpdate.InstitutionID);
            return View(reviewToUpdate);
        }




        // GET: /Review/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteReview(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: /Review/Delete/5
        [HttpPost, ActionName("DeleteReview")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmedReview(int id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
            db.SaveChanges();
            return RedirectToAction("AllReviews");
        }


        //------------Institution------------

        // GET: /Institution/
        [Authorize(Roles = "Admin")]
        public ActionResult IndexInstitution()
        {
            return View(db.Institutions.ToList());
        }

        // GET: /Institution/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult DetailsInstitution(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return HttpNotFound();
            }
            return View(institution);
        }

        // GET: /Institution/Create
        [Authorize(Roles = "Admin")]
        public ActionResult CreateInstitution()
        {
            return View();
        }

        // POST: /Institution/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateInstitution([Bind(Include = "InstitutionID,Name,Address,Description")] Institution institution)
        {
            if (ModelState.IsValid)
            {
                db.Institutions.Add(institution);
                db.SaveChanges();
                return RedirectToAction("IndexInstitution");
            }

            return View(institution);
        }

        // GET: /Institution/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult EditInstitution(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return HttpNotFound();
            }
            return View(institution);
        }

        // POST: /Institution/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult EditInstitution([Bind(Include = "InstitutionID,Name,Address,Description")] Institution institution)
        {
            if (ModelState.IsValid)
            {
                db.Entry(institution).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexInstitution");
            }
            return View(institution);
        }

        // GET: /Institution/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteInstitution(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return HttpNotFound();
            }
            return View(institution);
        }

        // POST: /Institution/Delete/5
        [HttpPost, ActionName("DeleteInstitution")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmedInstitution(int id)
        {
            Institution institution = db.Institutions.Find(id);
            db.Institutions.Remove(institution);
            db.SaveChanges();
            return RedirectToAction("IndexInstitution");
        }


        public ActionResult getGraphReviewPerYear()
        {
            List<Graph> graphs = new List<Graph>();
            var reviews = from d in db.Reviews select d;
            int reviews2013 = 0;
            int reviews2014 = 0;
            int reviews2015 = 0;
            foreach (var item in reviews)
            {
                if (item.StudyingYear == 2013)
                    reviews2013++;
                else if (item.StudyingYear == 2014)
                    reviews2014++;
                else if (item.StudyingYear == 2015)
                    reviews2015++;
            }

            graphs.Add(new Graph { State = "2013", freq = new Freq { LecturerReadine = reviews2013 } });
            graphs.Add(new Graph { State = "2014", freq = new Freq { LecturerReadine = reviews2014 } });
            graphs.Add(new Graph { State = "2015", freq = new Freq { LecturerReadine = reviews2015 } });

            return Json(graphs, JsonRequestBehavior.AllowGet);
        }

    }


}
