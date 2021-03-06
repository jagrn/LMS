﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.Models;
using LMS.Repositories;
using LMS.ViewModels;

namespace LMS.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class ActivitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Activities
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.Activities.ToList());
        }

        // GET: Activities/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: Activities/Manage/5
        public ActionResult Manage(int? id, int? moduleId, int? courseId, string getOperation, string viewMessage)
        {
            if ((getOperation == null) || (((id == null) || (id == 0)) && (getOperation != "New"))
                || (moduleId == 0) || (moduleId == null) || (courseId == 0) || (courseId == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ActivityViewModel viewModel = new ActivityViewModel();
            viewModel.CourseId = (int) courseId;
            viewModel.ModuleId = (int) moduleId;
            viewModel.PostMessage = viewMessage;

            // Load view model with activity specific info
            viewModel.ShowDocuments = false;
            if (getOperation == "New")
            {
                // Create new, reached from module views only              
                viewModel.StartDate = DateTime.Parse("2017-01-01");
                //viewModel.EndDate = DateTime.Parse("2017-01-01");
                viewModel.Deadline = DateTime.Parse("2017-01-01");
                viewModel.Period = SelectActivityPeriod.Heldag;                     // Deafult selection
                viewModel.SelectActivityType = SelectActivityType.Föreläsning;      // Default selection
            }
            else // getOperation == "Load"/"LoadMini"/"LoadAct"/"LoadDoc"
            {
                // Load existing, reached from module views only
                var singleActivity = ActivityRepo.RetrieveActivity(moduleId, id);
                if (singleActivity.repoResult == ActivityRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                viewModel.Id = singleActivity.activity.Id;
                viewModel.Name = singleActivity.activity.Name;
                viewModel.Description = singleActivity.activity.Description;
                viewModel.StartDate = singleActivity.activity.StartDate;
                //viewModel.EndDate = singleActivity.activity.EndDate;
                viewModel.Period = (SelectActivityPeriod) singleActivity.activity.ActivityPeriod;
                viewModel.SelectActivityType = (SelectActivityType) singleActivity.activity.ActivityType;
                viewModel.Deadline = singleActivity.activity.Deadline;

                viewModel.NoOfDocuments = DocumentRepo.RetrieveNoOfDocuments(courseId, moduleId, id);

                if ((getOperation == "Load") || (getOperation == "LoadDoc"))
                {
                    viewModel.ActivityDocuments = DocumentRepo.RetrieveCourseDocumentList(courseId, moduleId, id);
                    viewModel.ShowDocuments = true;
                }
                else // getOperation == "LoadMini"/"LoadMod"
                {
                    viewModel.ActivityDocuments = null;
                    viewModel.ShowDocuments = false;
                }
            }
            
            // Load view model with additional display info wrt parent module
            var moduleActivityList = ActivityRepo.RetrieveModuleActivityList(moduleId);
            if (moduleActivityList.repoResult == ActivityRepoResult.NotFound)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            viewModel.ModuleActivities = moduleActivityList.activityList;
            viewModel.ModuleName = ModuleRepo.RetrieveModuleName(moduleId);

            viewModel.AvailableTime = ActivityRepo.RetrieveActivityFreePeriods(moduleId);

            return View(viewModel);
        }

        // POST: Activities/Manage/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "Id,Name,Description,StartDate,EndDate,Period,Deadline,ActivityType,SelectActivityType,ModuleId,CourseId,ShowDocuments,PostNavigation,PostOperation,PostMessage")] ActivityViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // This code section is a copy from the activity handling in module management but
                // can be used as a template for document handling within an activity too.
                // When adding a new document, a SaveDoc PostNavigation should be filed with
                // a New PostOperation for the existing activity, where New refers to a
                // new document. To re-direct to the document management correctly the New cmd is temporarily
                // stored in a local variable, while PostOperation is set to Update for the existing
                // activity, which must exist for document to be attached to it.
                //var actPostOp = viewModel.PostOperation;        // PostOperation concerns activity, not module, in this case
                //if (viewModel.PostNavigation == "SaveAct")
                //{
                //    // Operation on module must be "Update" since parent module is required
                //    viewModel.PostOperation = "Update";
                //}

                if (((viewModel.Id == 0) && (viewModel.PostOperation == "Update")) || (viewModel.ModuleId == 0) || (viewModel.CourseId == 0))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (viewModel.PostOperation == "New")
                {
                    viewModel.Id = 0;
                }

                // Prepare start and end data for activity
                var start = viewModel.StartDate.ToShortDateString();
                var end = viewModel.StartDate.ToShortDateString();
                switch ((ActivityPeriod)viewModel.Period)
                {
                    case ActivityPeriod.AM:
                        start = start + " 08:30:00";
                        end = end + " 12:00:00";
                        break;
                    case ActivityPeriod.FM:
                        start = start + " 13:00:00";
                        end = end + " 16:30:00";
                        break;
                    case ActivityPeriod.FullDay:
                        start = start + " 08:30:00";
                        end = end + " 16:30:00";
                        break;
                    default:
                        break;
                }
                var StartPoint = DateTime.Parse(start);
                var EndPoint = DateTime.Parse(end);

                // Input validation
                var validMess = ActivityRepo.IsActivityNameValid(viewModel.ModuleId, viewModel.Id, viewModel.Name);
                if (validMess == null)
                {
                    validMess = ActivityRepo.IsActivitySpanInModule(viewModel.CourseId, viewModel.ModuleId, StartPoint, EndPoint);
                }
                if (validMess == null)
                {
                    validMess = ActivityRepo.IsActivitySpanValid(viewModel.ModuleId, viewModel.Id, StartPoint, EndPoint);
                }                
                if (validMess != null)
                {
                    // Load view model with additional display info wrt parent module
                    var moduleActivityList = ActivityRepo.RetrieveModuleActivityList(viewModel.ModuleId);
                    if (moduleActivityList.repoResult == ActivityRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    viewModel.ModuleActivities = moduleActivityList.activityList;
                    viewModel.ModuleName = ModuleRepo.RetrieveModuleName(viewModel.ModuleId);

                    viewModel.PostMessage = validMess;
                    return View(viewModel);
                }
                // End of input validation

                // Create the activity prototype
                Activity activity = new Activity();
                activity.Name = viewModel.Name;
                activity.Description = viewModel.Description;
                activity.ActivityPeriod = (ActivityPeriod)viewModel.Period;

                // Update StartDate time and EndDate time wrt selected Period               
                activity.StartDate = StartPoint;
                activity.EndDate = EndPoint;

                activity.ActivityType = (ActivityType) viewModel.SelectActivityType;
                activity.Deadline = DateTime.Parse("2017-01-01");                       // Dummy init so far
                activity.ModuleId = viewModel.ModuleId;

                // Perform Add or Update operation against DB
                if (viewModel.PostOperation == "New")
                {
                    viewModel.Id = ActivityRepo.AddActivity(activity);
                    viewModel.PostMessage = "Den nya aktiviteten " + viewModel.Name + " är sparad";
                }
                if (viewModel.PostOperation == "Update")
                {
                    activity.Id = viewModel.Id;         // Use concerned id from view
                    ActivityRepoResult result = ActivityRepo.UpdateActivity(activity);
                    if (result == ActivityRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    viewModel.PostMessage = "Aktiviteten " + viewModel.Name + " är uppdaterad";
                }

                if (viewModel.PostNavigation == "Save")
                {
                    // Load view model with additional display info wrt parent module
                    var moduleActivityList = ActivityRepo.RetrieveModuleActivityList(viewModel.ModuleId);
                    if (moduleActivityList.repoResult == ActivityRepoResult.NotFound)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    viewModel.ModuleActivities = moduleActivityList.activityList;
                    viewModel.ModuleName = ModuleRepo.RetrieveModuleName(viewModel.ModuleId);

                    viewModel.AvailableTime = ActivityRepo.RetrieveActivityFreePeriods(viewModel.ModuleId);

                    if ((viewModel.PostOperation == "Update") && (viewModel.ShowDocuments))
                    {
                        // Load view model with additional display info wrt activity documents
                        viewModel.ActivityDocuments = DocumentRepo.RetrieveCourseDocumentList(viewModel.CourseId, viewModel.ModuleId, viewModel.Id);
                    }

                    viewModel.NoOfDocuments = DocumentRepo.RetrieveNoOfDocuments(viewModel.CourseId, viewModel.ModuleId, viewModel.Id);
                }

                switch (viewModel.PostNavigation)
                {
                    case "Save":
                        return View(viewModel);
                    case "SaveRet":
                        return RedirectToAction("Manage", "Modules", new { id = viewModel.ModuleId, courseId = viewModel.CourseId, getOperation = "Load" });
                    // Prototype for the new SaveDoc case
                    //case "SaveDoc":
                    //    return RedirectToAction("Manage", "Activities", new { moduleId = viewModel.Id });
                    ////break;
                    default:
                        break;
                }
            }
            viewModel.PostMessage = "ERROR: Misslyckad POST operation for aktivitet";
            return View(viewModel);
        }

        // GET: Activities/Delete/5
        public ActionResult Delete(int? id, int? moduleId, int? courseId, string deleteType)
        {
            if ((deleteType == null) || (((id == null) || (id == 0)) && (deleteType == "Single"))
                || (moduleId == null) || (moduleId == 0) || (courseId == null) || (courseId == 0))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ActivityDeleteViewModel viewModel = new ActivityDeleteViewModel();
            viewModel.CourseId = (int) courseId;
            viewModel.ModuleId = (int) moduleId;
            viewModel.DeleteType = deleteType;

            if (deleteType == "Single")
            {
                // Init specifically required fields
                var singleActivity = ActivityRepo.RetrieveActivity(moduleId, id);
                if (singleActivity.repoResult == ActivityRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                viewModel.Id = singleActivity.activity.Id;
                viewModel.Name = singleActivity.activity.Name;
                viewModel.Description = singleActivity.activity.Description;
                viewModel.StartDate = singleActivity.activity.StartDate;
                //viewModel.EndDate = singleActivity.activity.EndDate;
                viewModel.Period = (SelectActivityPeriod)singleActivity.activity.ActivityPeriod;
                viewModel.SelectActivityType = (SelectActivityType) singleActivity.activity.ActivityType;
                viewModel.Deadline = singleActivity.activity.Deadline;
            }
            if (deleteType == "All")
            {
                // Init specifically required fields
                var moduleActivityList = ActivityRepo.RetrieveModuleActivityList(moduleId);
                if (moduleActivityList.repoResult == ActivityRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                viewModel.ModuleActivities = moduleActivityList.activityList;
            }                    
            return View(viewModel);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id, int? moduleId, int? courseId, string deleteType)
        {
            if ((deleteType == null) || (((id == null) || (id == 0)) && (deleteType == "Single"))
                || (moduleId == null) || (moduleId == 0) || (courseId == null) || (courseId == 0))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ActivityRepoResult result;
            string message = "";

            if (deleteType == "Single")
            {         
                string activityName = ActivityRepo.RetrieveActivityName(id);
                message = "Aktiviteten " + @activityName + " är borttagen";
                result = ActivityRepo.RemoveActivity(moduleId, id);            
            }
            else // deleteType == "All"
            {
                message = "Alla aktiviteter är borttagna för aktuell modul";
                result =  ActivityRepo.RemoveModuleActivities(moduleId);
            }

            if (result == ActivityRepoResult.NotFound)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return RedirectToAction("Manage", "Activities", new { moduleId = moduleId, courseId = courseId, getOperation = "New", viewMessage = message });
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
