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

namespace LMS.Controllers
{
    public class ActivitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Activities
        public ActionResult Index()
        {
            return View(db.Activities.ToList());
        }

        // GET: Activities/Details/5
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

        // GET: Activities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,Deadline,ActitvityType,ModuleId")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(activity);
        }

        // GET: Activities/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,Deadline,ActitvityType,ModuleId")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activity);
        }

        // GET: Activities/Manage/5
        public ActionResult Manage(int? id, int? moduleId, int? courseId, string viewMessage)
        {
            if ((moduleId == 0) || (moduleId == null) || (courseId == 0) || (courseId == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityViewModel viewModel = new ActivityViewModel();
            viewModel.CourseId = (int) courseId;
            viewModel.PostMessage = viewMessage;

            if ((id == null) || (id == 0))
            {
                // Create new, reached from module views only
                viewModel.ModuleId = (int)moduleId;
                viewModel.StartDate = DateTime.Parse("2017-01-01");
                viewModel.EndDate = DateTime.Parse("2017-01-01");
                viewModel.Deadline = DateTime.Parse("2017-01-01");
                viewModel.ActitvityType = ActivityType.E_Learning;      
            }
            else
            {
                // Edit existing, reached from module views only
                Activity activity = new Activity();
                activity = db.Activities.Find(id);
                if (activity == null)
                {
                    return HttpNotFound();
                }
                viewModel.Id = activity.Id;
                viewModel.Name = activity.Name;
                viewModel.Description = activity.Description;
                viewModel.StartDate = activity.StartDate;
                viewModel.EndDate = activity.EndDate;
                viewModel.Deadline = activity.Deadline;
                viewModel.ActitvityType = activity.ActitvityType;
                viewModel.ModuleId = activity.ModuleId;
            }

            // Load view model with additional display info wrt parent module
            Module module = new Models.Module();
            module = db.Modules.Find(moduleId);
            viewModel.ModuleActivities = new List<ActivityListData>();
            foreach (var mod in module.Activities)
            {
                ActivityListData moduleActivity = new ActivityListData();
                moduleActivity.Id = mod.Id;
                moduleActivity.Name = mod.Name;
                viewModel.ModuleActivities.Add(moduleActivity);
            }
            viewModel.ModuleName = module.Name;
            return View(viewModel);
        }


        // POST: Activities/Manage/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "Id,Name,Description,StartDate,EndDate,Deadline,ActivityType,ModuleId,CourseId,PostNavigation,PostOperation,PostMessage")] ActivityViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Can be used as template for document model
                //var actPostOp = viewModel.PostOperation;        // PostOperation concerns activity, not module, in this case
                //if (viewModel.PostNavigation == "SaveAct")
                //{
                //    // Operation on module must be "Update" since parent module is required
                //    viewModel.PostOperation = "Update";
                //}

                // Create the activity prototype for saving
                Activity activity = new Activity();
                activity.Name = viewModel.Name;
                activity.Description = viewModel.Description;
                activity.StartDate = viewModel.StartDate;
                activity.EndDate = viewModel.EndDate;
                activity.ActitvityType = viewModel.ActitvityType;
                activity.Deadline = viewModel.Deadline;
                activity.ModuleId = viewModel.ModuleId;

                // Perform Add or Update operation against DB
                if (viewModel.PostOperation == "New")
                {
                    // Create new
                    db.Activities.Add(activity);
                    db.SaveChanges();
                    viewModel.Id = activity.Id;       // Use newly checked out id to view
                    viewModel.PostMessage = "The new " + viewModel.Name + " activity was successfully saved";
                }
                if (viewModel.PostOperation == "Update")
                {
                    if (viewModel.Id == 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    // Edit existing
                    activity.Id = viewModel.Id;       // Use concerned id from view
                    db.Entry(activity).State = EntityState.Modified;
                    db.SaveChanges();
                    viewModel.PostMessage = "The " + viewModel.Name + " activity was successfully updated";
                }

                if (viewModel.PostNavigation == "Save")
                {
                    // Load view model with additional display info from parent module
                    Module module = new Models.Module();
                    module = db.Modules.Find(viewModel.ModuleId);
                    viewModel.ModuleActivities = new List<ActivityListData>();
                    foreach (var act in module.Activities)
                    {
                        ActivityListData moduleActivity = new ActivityListData();
                        moduleActivity.Id = act.Id;
                        moduleActivity.Name = act.Name;
                        viewModel.ModuleActivities.Add(moduleActivity);
                    }
                    viewModel.ModuleName = module.Name;
                }

                switch (viewModel.PostNavigation)
                {
                    case "Save":
                        return View(viewModel);
                    //break;
                    case "SaveRet":
                        return RedirectToAction("Manage", "Modules", new { id = viewModel.ModuleId, courseId = viewModel.CourseId });
                    //break;
                    //case "SaveAct":
                    //    return RedirectToAction("Manage", "Activities", new { moduleId = viewModel.Id });
                    ////break;
                    default:
                        break;
                }
            }
            viewModel.PostMessage = "Error (model state invalid), no module saving performed";
            return View(viewModel);
        }

        // GET: Activities/Delete/5
        public ActionResult Delete(int? id, int? moduleId, int? courseId, string deleteType)
        {
            if (((id == null) && (deleteType == "Single")) || (moduleId == null) || (courseId == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ActivityDeleteViewModel viewModel = new ActivityDeleteViewModel();
            if (deleteType == "Single")
            {
                Activity activity = db.Activities.Find(id);
                if (activity == null)
                {
                    return HttpNotFound();
                }

                // Init specifically required fields
                viewModel.Id = activity.Id;
                viewModel.Name = activity.Name;
                viewModel.Description = activity.Description;
                viewModel.StartDate = activity.StartDate;
                viewModel.EndDate = activity.EndDate;
                viewModel.ActitvityType = activity.ActitvityType;
                viewModel.Deadline = activity.Deadline;
            }
            if (deleteType == "All")
            {
                // Init specifically required fields
                Module module = new Models.Module();
                module = db.Modules.Find(moduleId);
                viewModel.ModuleActivities = new List<ActivityListData>();
                foreach (var act in module.Activities)
                {
                    ActivityListData moduleActivity = new ActivityListData();
                    moduleActivity.Id = act.Id;
                    moduleActivity.Name = act.Name;
                    viewModel.ModuleActivities.Add(moduleActivity);
                }
            }
            viewModel.DeleteType = deleteType;
            viewModel.ModuleId = (int) moduleId;
            viewModel.CourseId = (int)courseId;
            return View(viewModel);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id, int? moduleId, int? courseId, string deleteType)
        {
            if (deleteType == "Single")
            {
                Activity activity = db.Activities.Find(id);
                var activityName = activity.Name;
                db.Activities.Remove(activity);
                db.SaveChanges();
                return RedirectToAction("Manage", "Activities", new { moduleId = moduleId, courseId = courseId, viewMessage = "The " + @activityName + " activity was removed" });
            }
            else // deleteType == "All"
            {
                Module module = new Models.Module();
                module = db.Modules.Find(moduleId);
                var moduleActivityIds = new List<int>();
                foreach (var act in module.Activities)
                {
                    moduleActivityIds.Add(act.Id);
                }
                foreach (var actId in moduleActivityIds)
                {
                    Activity activity = db.Activities.Find(actId);
                    db.Activities.Remove(activity);
                    db.SaveChanges();
                }
                return RedirectToAction("Manage", "Activities", new { moduleId = moduleId, courseId = courseId, viewMessage = "All modules removed" });
            }
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
