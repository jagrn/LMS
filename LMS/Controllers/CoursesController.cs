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
        public ActionResult Manage(int? id, string getOperation, string viewMessage)
        {
            if ((getOperation == null) || (((id == null) || (id == 0)) && (getOperation == "Load")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CourseViewModel viewModel = new CourseViewModel();
            viewModel.PostMessage = viewMessage;

            // Load view model with module specific info
            if (getOperation == "New")
            {
                // Create new, reached from course views only              
                viewModel.StartDate = DateTime.Parse("2017-01-01");
                viewModel.EndDate = DateTime.Parse("2017-12-31");
            }
            else // getOperation == "Load"
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
                var courseModuleList_ = ModuleRepo.RetrieveCourseModuleList(id);
                if (courseModuleList_.repoResult == ModuleRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                viewModel.CourseModules = courseModuleList_.moduleList;
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

        // POST: Modules/Manage/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "Id,Name,Description,StartDate,EndDate,ModuleId,PostNavigation,PostOperation,PostMessage")] CourseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var actPostOp = viewModel.PostOperation;        // PostOperation concerns activity, not module, in this case
                if (viewModel.PostNavigation == "SaveAct")
                {
                    // Operation on module must be "Update" since parent module is required
                    viewModel.PostOperation = "Update";
                }

                if (  ( (viewModel.Id == 0) && (viewModel.PostOperation == "Update") )  )
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Create the module prototype             
                Course course = new Course();
                course.Name = viewModel.Name;
                course.Description = viewModel.Description;
                course.StartDate = viewModel.StartDate;
                course.EndDate = viewModel.EndDate;

                // Perform Add or Update operation against DB
                if (viewModel.PostOperation == "New")
                {
                    viewModel.Id = CourseRepo.AddCourse(course);
                    viewModel.PostMessage = "The new course " + viewModel.Name + " was successfully saved";
                }
                if (viewModel.PostOperation == "Update")
                {
                    course.Id = viewModel.Id;       // Use concerned id from view
                    CourseRepoResult result = CourseRepo.UpdateCourse(course);
                    if (result == CourseRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    viewModel.PostMessage = "The course " + viewModel.Name + " was successfully updated";
                }

                if (viewModel.PostNavigation == "Save")
                {
                    // Load view model with additional display info wrt parent course               
                    var allCoursesList = CourseRepo.RetrieveCourseList();
                    if (allCoursesList.repoResult == CourseRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    viewModel.AllCourses = allCoursesList.courseList;

                    if (viewModel.PostOperation == "Update")
                    {
                        // Load view model with additional display info wrt module activities
                        var courseModuleList = ModuleRepo.RetrieveCourseModuleList(viewModel.Id);
                        if (courseModuleList.repoResult == ModuleRepoResult.NotFound)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                        }
                        viewModel.CourseModules = courseModuleList.moduleList;
                    }
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
            viewModel.PostMessage = "Error (model state invalid), no module saving performed";
            return View(viewModel);
        }


        // GET: Courses/Delete/5
        public ActionResult Delete(int? id, string deleteType)
        {
            if ((deleteType == null) || (((id == null) || (id == 0)) && (deleteType == "Single")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CourseDeleteViewModel viewModel = new CourseDeleteViewModel();
            viewModel.Id = (int) id;
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
                message = "The " + courseName + " course was removed";
                result = CourseRepo.RemoveCourse(id);
            }
            else // deleteType == "All"
            {
                message = "All modules removed";
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
