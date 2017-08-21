using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.Models;
using LMS.ViewModels;
using LMS.Repositories;

namespace LMS.Controllers
{
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Courses
        [Authorize(Roles = "Teacher")]
        public ActionResult Index()
        {
            return View(db.Courses.ToList());
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
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

        // GET: Courses/Manage/5
        [Authorize(Roles = "Teacher")]
        public ActionResult Manage(int? id, string getOperation, string viewMessage)
        {
            if ((getOperation == null) || (((id == null) || (id == 0)) && (getOperation == "Load")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CourseViewModel viewModel = new CourseViewModel();
            viewModel.PostMessage = viewMessage;

            // Load view model with module specific info
            viewModel.ShowModules = false;
            viewModel.ShowDocuments = false;
            if (getOperation == "New")
            {
                // Create new, reached from course views only              
                viewModel.StartDate = DateTime.Parse("2017-01-01");
                viewModel.EndDate = DateTime.Parse("2017-01-01");
            }           
            else // getOperation == "Load"/"LoadMini"/"LoadMod"/"LoadDoc"
            {
                // Load existing, reached from course views only
                var singleCourse = CourseRepo.RetrieveCourse(id);
                if (singleCourse.repoResult == CourseRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                viewModel.Id = singleCourse.course.Id;
                viewModel.Name = singleCourse.course.Name;
                viewModel.Description = singleCourse.course.Description;
                viewModel.StartDate = singleCourse.course.StartDate;
                viewModel.EndDate = singleCourse.course.EndDate;
                
                // Load view model with additional display info wrt module activities
                var courseModuleList = ModuleRepo.RetrieveCourseModuleList(id);
                if (courseModuleList.repoResult == ModuleRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                viewModel.NoOfModules = courseModuleList.moduleList.Count;
                viewModel.NoOfDocuments = DocumentRepo.RetrieveNoODocuments(id, null, null);

                if ((getOperation == "Load") || (getOperation == "LoadMod"))
                {
                    viewModel.CourseModules = courseModuleList.moduleList;
                    viewModel.ShowModules = true;
                }
                else // getOperation == "LoadMini"/"LoadDoc"
                {
                    viewModel.CourseModules = null;
                    viewModel.ShowModules = false;
                }
                if ((getOperation == "Load") || (getOperation == "LoadDoc"))
                {
                    viewModel.CourseDocuments = DocumentRepo.RetrieveCourseDocumentList(id, null, null);                   
                    viewModel.ShowDocuments = true;
                }
                else // getOperation == "LoadMini"/"LoadMod"
                {
                    viewModel.CourseDocuments = null;
                    viewModel.ShowDocuments = false;
                }

            }

            // Load view model with additional display info wrt parent course
            var allCoursesList = CourseRepo.RetrieveCourseList();
            if (allCoursesList.repoResult == CourseRepoResult.NotFound)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            viewModel.AllCourses = allCoursesList.courseList;

            return View(viewModel);
        }

        // POST: Cources/Manage/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "Id,Name,Description,StartDate,EndDate,ModuleId,ShowModules,ShowDocuments,PostNavigation,PostOperation,PostMessage")] CourseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var actPostOp = viewModel.PostOperation;        // PostOperation concerns activity, not module, in this case
                if (viewModel.PostNavigation == "SaveAct")
                {
                    // Operation on course must be "Update" since parent course is required
                    viewModel.PostOperation = "Update";
                }

                if ((viewModel.Id == 0) && (viewModel.PostOperation == "Update"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (viewModel.PostOperation == "New")
                {
                    viewModel.Id = 0;
                }

                // Input validation
                var validMess = CourseRepo.IsCourseNameValid(viewModel.Id, viewModel.Name);
                if (validMess != null)
                {
                    // Load view model with additional display info wrt all courses               
                    var allCoursesList = CourseRepo.RetrieveCourseList();
                    if (allCoursesList.repoResult == CourseRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    viewModel.AllCourses = allCoursesList.courseList;

                    // Load view model with additional display info wrt module activities
                    var courseModuleList = ModuleRepo.RetrieveCourseModuleList(viewModel.Id);
                    if (courseModuleList.repoResult == ModuleRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    viewModel.CourseModules = courseModuleList.moduleList;

                    viewModel.PostMessage = validMess;
                    return View(viewModel);
                }
                // End of input validation

                // Create the course prototype             
                Course course = new Course();
                course.Name = viewModel.Name;
                course.Description = viewModel.Description;

                // Perform Add or Update operation against DB
                if (viewModel.PostOperation == "New")
                {
                    CourseModulesSpan courseSpan = ModuleRepo.RetrieveCourseSpan(0);
                    course.StartDate = courseSpan.start;
                    course.EndDate = courseSpan.end;
                    viewModel.Id = CourseRepo.AddCourse(course);
                    viewModel.PostMessage = "Den nya kursen " + viewModel.Name + " är sparad";
                }
                if (viewModel.PostOperation == "Update")
                {
                    course.Id = viewModel.Id;       // Use concerned id from view
                    CourseModulesSpan courseSpan = ModuleRepo.RetrieveCourseSpan(course.Id);
                    course.StartDate = courseSpan.start;
                    course.EndDate = courseSpan.end;              
                    CourseRepoResult result = CourseRepo.UpdateCourse(course);
                    if (result == CourseRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    viewModel.PostMessage = "Kursen " + viewModel.Name + " är uppdaterad";
                }

                if (viewModel.PostNavigation == "Save")
                {
                    // Load view model with additional display info wrt all courses               
                    var allCoursesList = CourseRepo.RetrieveCourseList();
                    if (allCoursesList.repoResult == CourseRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    viewModel.AllCourses = allCoursesList.courseList;

                    if ((viewModel.PostOperation == "Update") && (viewModel.ShowModules))
                    {
                        // Load view model with additional display info wrt course modules
                        var courseModuleList = ModuleRepo.RetrieveCourseModuleList(viewModel.Id);
                        if (courseModuleList.repoResult == ModuleRepoResult.NotFound)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                        }
                        viewModel.CourseModules = courseModuleList.moduleList;
                    }

                    viewModel.NoOfModules = CourseRepo.RetrieveNoOfModules(viewModel.Id);
                }

                switch (viewModel.PostNavigation)
                {
                    case "Save":
                        return View(viewModel);
                    case "SaveRet":
                        return RedirectToAction("Index", "Courses");
                    case "SaveAct":
                        string actGetOp = "New";
                        if (actPostOp == "Update")
                            actGetOp = "Load";
                        return RedirectToAction("Manage", "Modules", new { id = viewModel.ModuleId, courseId = viewModel.Id, getOperation = actGetOp });
                    default:
                        break;
                }
            }
            viewModel.PostMessage = "ERROR: Misslyckad POST operation for kurs";
            return View(viewModel);
        }


        // GET: Courses/Delete/5
        [Authorize(Roles = "Teacher")]
        public ActionResult Delete(int? id, string deleteType)
        {
            if ((deleteType == null) || (((id == null) || (id == 0)) && (deleteType == "Single")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CourseDeleteViewModel viewModel = new CourseDeleteViewModel();
            viewModel.DeleteType = deleteType;

            if (deleteType == "Single")
            {
                // Init specifically required fields
                var singleCourse = CourseRepo.RetrieveCourse(id);
                if (singleCourse.repoResult == CourseRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                viewModel.Id = singleCourse.course.Id;
                viewModel.Name = singleCourse.course.Name;
                viewModel.Description = singleCourse.course.Description;
                viewModel.StartDate = singleCourse.course.StartDate;
                viewModel.EndDate = singleCourse.course.EndDate;
            }
            if (deleteType == "All")
            {
                // Init specifically required fields
                var allCoursesList = CourseRepo.RetrieveCourseList();
                if (allCoursesList.repoResult == CourseRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                viewModel.AllCourses = allCoursesList.courseList;
            }

            return View(viewModel);
        }

        // POST: Courses/Delete/5
        [Authorize(Roles = "Teacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id, string deleteType)
        {
            if ((deleteType == null) || (((id == null) || (id == 0)) && (deleteType == "Single")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CourseRepoResult result;
            string message = "";

            if (deleteType == "Single")
            {
                string courseName = CourseRepo.RetrieveCourseName(id);
                message = "Kursen " + courseName + " är borttagen";
                result = CourseRepo.RemoveCourse(id);
            }
            else // deleteType == "All"
            {
                message = "Alla kurser är borttagna";
                result = CourseRepo.RemoveCourses();
            }

            if (result == CourseRepoResult.NotFound)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return RedirectToAction("Manage", "Courses", new { getOperation = "New", viewMessage = message });
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
