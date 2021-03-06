﻿using System;
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
            if ((getOperation == null) || (((id == null) || (id == 0)) && (getOperation != "New"))
                || (courseId == 0) || (courseId == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModuleViewModel viewModel = new ModuleViewModel();
            viewModel.CourseId = (int)courseId;
            viewModel.PostMessage = viewMessage;

            // Load view model with module specific info
            viewModel.ShowActivities = false;
            viewModel.ShowDocuments = false;
            if (getOperation == "New")
            {
                // Create new, reached from course views only              
                viewModel.StartDate = DateTime.Parse("2017-01-01");
                viewModel.EndDate = DateTime.Parse("2017-01-01");
            }
            else // getOperation == "Load"/"LoadMini"/"LoadAct"/"LoadDoc"
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
                viewModel.NoOfActivities = moduleActivityList.activityList.Count;
                viewModel.NoOfDocuments = DocumentRepo.RetrieveNoOfDocuments(courseId, id, null);

                if ((getOperation == "Load") || (getOperation == "LoadAct"))
                {
                    viewModel.ModuleActivities = moduleActivityList.activityList;
                    viewModel.ShowActivities = true;
                }
                else // getOperation == "LoadMini"
                {
                    viewModel.ModuleActivities = null;
                    viewModel.ShowActivities = false;
                }

                if ((getOperation == "Load") || (getOperation == "LoadDoc"))
                {
                    viewModel.ModuleDocuments = DocumentRepo.RetrieveCourseDocumentList(courseId, id, null);
                    viewModel.ShowDocuments = true;
                }
                else // getOperation == "LoadMini"/"LoadMod"
                {
                    viewModel.ModuleDocuments = null;
                    viewModel.ShowDocuments = false;
                }

            }

            // Load view model with additional display info wrt parent course
            var courseModuleList = ModuleRepo.RetrieveCourseModuleList(courseId);
            if (courseModuleList.repoResult == ModuleRepoResult.NotFound)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            viewModel.CourseModules = courseModuleList.moduleList;
            viewModel.CourseName = CourseRepo.RetrieveCourseName(courseId);

            viewModel.AvailableTime = ModuleRepo.RetrieveModuleFreePeriods(courseId);

            return View(viewModel);
        }

        // POST: Modules/Manage/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId,ActivityId,ShowActivities,ShowDocuments,PostNavigation,PostOperation,PostMessage")] ModuleViewModel viewModel)
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

                if (viewModel.PostOperation == "New")
                {
                    viewModel.Id = 0;
                }

                // Prepare start and end data for module
                var start = viewModel.StartDate.ToShortDateString();
                var end = viewModel.EndDate.ToShortDateString();
                start = start + " 08:30:00";
                end = end + " 16:30:00";
                viewModel.StartDate = DateTime.Parse(start);
                viewModel.EndDate = DateTime.Parse(end);


                // Input validation
                var validMess = ModuleRepo.IsModuleNameValid(viewModel.CourseId, viewModel.Id, viewModel.Name);
                if (validMess == null)
                {
                    if (viewModel.StartDate >= viewModel.EndDate)
                        validMess = "Modulens start ligger efter modulens slut eller samtidigt";
                }
                if (validMess == null)
                {
                    validMess = ModuleRepo.IsModuleSpanValid(viewModel.CourseId, viewModel.Id, viewModel.StartDate, viewModel.EndDate);
                }
                if (validMess != null)
                {
                    // Load view model with additional display info wrt parent course               
                    var courseModuleList = ModuleRepo.RetrieveCourseModuleList(viewModel.CourseId);
                    if (courseModuleList.repoResult == ModuleRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    viewModel.CourseModules = courseModuleList.moduleList;
                    viewModel.CourseName = CourseRepo.RetrieveCourseName(viewModel.CourseId);

                    // Load view model with additional display info wrt module activities
                    var moduleActivityList = ActivityRepo.RetrieveModuleActivityList(viewModel.Id);
                    if (moduleActivityList.repoResult == ActivityRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    viewModel.ModuleActivities = moduleActivityList.activityList;

                    viewModel.AvailableTime = ModuleRepo.RetrieveModuleFreePeriods(viewModel.CourseId);
                    viewModel.PostMessage = validMess;
                    return View(viewModel);
                }
                // End of input validation

                // Create the module prototype             
                Module module = new Module();
                module.Name = viewModel.Name;
                module.Description = viewModel.Description;
                module.StartDate = viewModel.StartDate;
                module.EndDate = viewModel.EndDate;
                module.CourseId = viewModel.CourseId;

                // Perform Add or Update operation against DB
                if (viewModel.PostOperation == "New")
                {
                    viewModel.Id = ModuleRepo.AddModule(module);
                    viewModel.PostMessage = "Den nya modulen " + viewModel.Name + " är sparad";
                }
                if (viewModel.PostOperation == "Update")
                {
                    module.Id = viewModel.Id;       // Use concerned id from view
                    ModuleRepoResult result = ModuleRepo.UpdateModule(module);
                    if (result == ModuleRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }       
                    viewModel.PostMessage = "Modulen " + viewModel.Name + " är uppdaterad";
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

                    if ((viewModel.PostOperation == "Update") && (viewModel.ShowActivities))
                    {
                        // Load view model with additional display info wrt module activities
                        var moduleActivityList = ActivityRepo.RetrieveModuleActivityList(viewModel.Id);
                        if (moduleActivityList.repoResult == ActivityRepoResult.NotFound)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                        }
                        viewModel.ModuleActivities = moduleActivityList.activityList;
                    }

                    viewModel.NoOfActivities = ModuleRepo.RetrieveNoOfActivities(viewModel.Id);

                    if ((viewModel.PostOperation == "Update") && (viewModel.ShowDocuments))
                    {
                        // Load view model with additional display info wrt module documents
                        viewModel.ModuleDocuments = DocumentRepo.RetrieveCourseDocumentList(viewModel.CourseId, viewModel.Id, null);
                    }

                    viewModel.NoOfDocuments = DocumentRepo.RetrieveNoOfDocuments(viewModel.CourseId, viewModel.Id, null);
                }

                // Adjust the course span according to posted module
                CourseRepoResult adjustResult = CourseRepo.UpdateCourseSpan(viewModel.CourseId);           
                if (adjustResult == CourseRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                if (viewModel.PostNavigation == "Save")
                {
                    viewModel.AvailableTime = ModuleRepo.RetrieveModuleFreePeriods(viewModel.CourseId);
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
            viewModel.PostMessage = "ERROR: Misslyckad POST operation for modul";
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
                message = "Modulen " + moduleName + " är borttagen";
                result = ModuleRepo.RemoveModule(courseId, id);
            }
            else // deleteType == "All"
            {
                message = "Alla moduler är borttagna för aktuell kurs";
                result = ModuleRepo.RemoveCourseModules(courseId);
            }

            if (result == ModuleRepoResult.NotFound)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Adjust the course span according to removed module(s)
            CourseRepoResult adjustResult = CourseRepo.UpdateCourseSpan(courseId);
            if (adjustResult == CourseRepoResult.NotFound)
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
