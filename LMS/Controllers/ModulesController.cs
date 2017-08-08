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

    [Authorize(Roles = "Teacher")]
    public class ModulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Modules
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.Modules.ToList());
        }

        // GET: Modules/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // GET: Modules/Manage/5
        public ActionResult Manage(int? id, int? courseId, string getOperation, string viewMessage)
        {
            if ((getOperation == null) || (((id == null) || (id == 0)) && (getOperation == "Load"))
                || (courseId == 0) || (courseId == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModuleViewModel viewModel = new ModuleViewModel();
            viewModel.CourseId = (int)courseId;
            viewModel.PostMessage = viewMessage;

            // Load view model with module specific info
            if (getOperation == "New")
            {
                // Create new, reached from course views only              
                viewModel.StartDate = DateTime.Parse("2017-01-01");
                viewModel.EndDate = DateTime.Parse("2017-01-01");
            }
            else // getOperation == "Load"
            {
                // Load existing, reached from course views only
                var singleModule = ModuleRepo.RetrieveModule(courseId, id);
                if (singleModule.repoResult == ModuleRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                viewModel.Id = singleModule.module.Id;
                viewModel.Name = singleModule.module.Name;
                viewModel.Description = singleModule.module.Description;
                viewModel.StartDate = singleModule.module.StartDate;
                viewModel.EndDate = singleModule.module.EndDate;

                // Load view model with additional display info wrt module activities
                var moduleActivityList = ActivityRepo.RetrieveModuleActivityList(id);
                if (moduleActivityList.repoResult == ActivityRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                viewModel.ModuleActivities = moduleActivityList.activityList;
            }

            // Load view model with additional display info wrt parent course
            var courseModuleList = ModuleRepo.RetrieveCourseModuleList(courseId);
            if (courseModuleList.repoResult == ModuleRepoResult.NotFound)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            viewModel.CourseModules = courseModuleList.moduleList;
            viewModel.CourseName = CourseRepo.RetrieveCourseName(courseId);

            return View(viewModel);
        }

        // POST: Modules/Manage/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId,ActivityId,PostNavigation,PostOperation,PostMessage")] ModuleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var actPostOp = viewModel.PostOperation;        // PostOperation concerns activity, not module, in this case
                if (viewModel.PostNavigation == "SaveAct")
                {
                    // Operation on module must be "Update" since parent module is required
                    viewModel.PostOperation = "Update";
                }

                if (((viewModel.Id == 0) && (viewModel.PostOperation == "Update")) || (viewModel.CourseId == 0))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Create the module prototype             
                Module module = new Module();
                module.Name = viewModel.Name;
                module.Description = viewModel.Description;
                module.StartDate = viewModel.StartDate;
                module.EndDate = viewModel.EndDate;
                module.CourseId = viewModel.CourseId;

                var courseSpan = ModuleRepo.RetrieveCourseSpan(viewModel.CourseId);


                // Perform Add or Update operation against DB
                if (viewModel.PostOperation == "New")
                {
                    viewModel.Id = ModuleRepo.AddModule(module);
                    viewModel.PostMessage = "The new " + viewModel.Name + " module was successfully saved";
                }
                if (viewModel.PostOperation == "Update")
                {
                    module.Id = viewModel.Id;       // Use concerned id from view
                    ModuleRepoResult result = ModuleRepo.UpdateModule(module);
                    if (result == ModuleRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }       
                    viewModel.PostMessage = "The " + viewModel.Name + " module was successfully updated";
                }

                if (viewModel.PostNavigation == "Save")
                {
                    // Load view model with additional display info wrt parent course               
                    var courseModuleList = ModuleRepo.RetrieveCourseModuleList(viewModel.CourseId);
                    if (courseModuleList.repoResult == ModuleRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    viewModel.CourseModules = courseModuleList.moduleList;
                    viewModel.CourseName = CourseRepo.RetrieveCourseName(viewModel.CourseId);

                    if (viewModel.PostOperation == "Update")
                    {
                        // Load view model with additional display info wrt module activities
                        var moduleActivityList = ActivityRepo.RetrieveModuleActivityList(viewModel.Id);
                        if (moduleActivityList.repoResult == ActivityRepoResult.NotFound)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                        }
                        viewModel.ModuleActivities = moduleActivityList.activityList;
                    }
                }
 
                switch (viewModel.PostNavigation)
                {
                    case "Save":
                        return View(viewModel);
                    case "SaveRet":
                        return RedirectToAction("Manage", "Courses", new { id = viewModel.CourseId, getOperation = "Load" });
                    case "SaveAct":
                        string actGetOp = "New";
                        if (actPostOp == "Update")
                            actGetOp = "Load";
                        return RedirectToAction("Manage", "Activities", new { id = viewModel.ActivityId, moduleId = viewModel.Id, courseId = viewModel.CourseId, getOperation = actGetOp });
                    default:
                        break;
                }
            }
            viewModel.PostMessage = "Error (model state invalid), no module saving performed";
            return View(viewModel);
        }

        // GET: Modules/Delete/5
        public ActionResult Delete(int? id, int? courseId, string deleteType)
        {
            if ((deleteType == null) || (((id == null) || (id == 0)) && (deleteType == "Single"))
                || (courseId == null) || (courseId == 0))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModuleDeleteViewModel viewModel = new ModuleDeleteViewModel();
            viewModel.CourseId = (int)courseId;
            viewModel.DeleteType = deleteType;

            if (deleteType == "Single")
            {
                // Init specifically required fields
                var singleModule = ModuleRepo.RetrieveModule(courseId, id);
                if (singleModule.repoResult == ModuleRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                viewModel.Id = singleModule.module.Id;
                viewModel.Name = singleModule.module.Name;
                viewModel.Description = singleModule.module.Description;
                viewModel.StartDate = singleModule.module.StartDate;
                viewModel.EndDate = singleModule.module.EndDate;
            }
            if (deleteType == "All")
            {
                // Init specifically required fields
                var courseModuleList = ModuleRepo.RetrieveCourseModuleList(courseId);
                if (courseModuleList.repoResult == ModuleRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                viewModel.CourseModules = courseModuleList.moduleList;
            }        
            
            return View(viewModel);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id, int? courseId, string deleteType)
        {
            if ((deleteType == null) || (((id == null) || (id == 0)) && (deleteType == "Single"))
                || (courseId == null) || (courseId == 0))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModuleRepoResult result;
            string message = "";

            if (deleteType == "Single")
            {
                string moduleName = ModuleRepo.RetrieveModuleName(id);
                message = "The " + moduleName + " activity was removed";
                result = ModuleRepo.RemoveModule(courseId, id);
            }
            else // deleteType == "All"
            {
                message = "All modules removed";
                result = ModuleRepo.RemoveCourseModules(courseId);
            }

            if (result == ModuleRepoResult.NotFound)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return RedirectToAction("Manage", "Modules", new { courseId = courseId, getOperation = "New", viewMessage = message });
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
