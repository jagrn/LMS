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
    public class ModulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Modules
        public ActionResult Index()
        {
            return View(db.Modules.ToList());
        }

        // GET: Modules/Details/5
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

        // GET: Modules/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId")] Module module)
        {
            if (ModelState.IsValid)
            {
                db.Modules.Add(module);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(module);
        }

        // GET: Modules/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Modules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId")] Module module)
        {
            if (ModelState.IsValid)
            {
                db.Entry(module).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(module);
        }

        // GET: Modules/Manage/5
        public ActionResult Manage(int? id, int? courseId, string viewMessage)
        {
            if ((courseId == 0) || (courseId == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleViewModel viewModel = new ModuleViewModel();
            viewModel.PostMessage = viewMessage;

            if ((id == null) || (id == 0))
            {
                // Create new, reached from course views only
                viewModel.CourseId = (int)courseId;
                viewModel.StartDate = DateTime.Parse("2017-01-01");
                viewModel.EndDate = DateTime.Parse("2017-01-01");
                viewModel.ModuleActivities = null;
            }
            else
            {
                // Edit existing, reached from course views only
                Module module = new Module();
                module = db.Modules.Find(id);
                if (module == null)
                {
                    return HttpNotFound();
                }
                viewModel.Id = module.Id;
                viewModel.Name = module.Name;
                viewModel.Description = module.Description;
                viewModel.StartDate = module.StartDate;
                viewModel.EndDate = module.EndDate;
                viewModel.CourseId = module.CourseId;             
                //viewModel.Activities = module.Activities;

                // Load view model with additional display info wrt activities
                viewModel.ModuleActivities = new List<ActivityListData>();
                foreach (var act in module.Activities)
                {
                    ActivityListData moduleActivity = new ActivityListData();
                    moduleActivity.Id = act.Id;
                    moduleActivity.Name = act.Name;
                    viewModel.ModuleActivities.Add(moduleActivity);
                }
            }

            // Load view model with additional display info wrt parent course
            Course course = new Models.Course();
            course = db.Courses.Find(courseId);
            viewModel.CourseModules = new List<ModuleListData>();           
            foreach (var mod in course.Modules)
            {
                ModuleListData courseModule = new ModuleListData();
                courseModule.Id = mod.Id;
                courseModule.Name = mod.Name;
                viewModel.CourseModules.Add(courseModule);
            }
            viewModel.CourseName = course.Name;

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

                // Create the module prototype for saving
                Module module = new Module();
                if ((viewModel.PostOperation == "Update") && (viewModel.Id != 0))
                {
                    module = db.Modules.Find(viewModel.Id);
                }
                module.Name = viewModel.Name;
                module.Description = viewModel.Description;
                module.StartDate = viewModel.StartDate;
                module.EndDate = viewModel.EndDate;
                module.CourseId = viewModel.CourseId;

                // Perform Add or Update operation against DB
                if (viewModel.PostOperation == "New")
                {
                    // Create new
                    db.Modules.Add(module);
                    db.SaveChanges();
                    viewModel.Id = module.Id;               // Use newly checked out id to view
                    viewModel.ModuleActivities = null;      // No activities yet for new module
                    viewModel.PostMessage = "The new " + viewModel.Name + " module was successfully saved";
                }
                if (viewModel.PostOperation == "Update")
                {
                    if (viewModel.Id == 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    
                    // Edit existing
                    module.Id = viewModel.Id;       // Use concerned id from view
                    db.Entry(module).State = EntityState.Modified;
                    db.SaveChanges();                 
                    viewModel.PostMessage = "The " + viewModel.Name + " module was successfully updated";
                }

                if (viewModel.PostNavigation == "Save")
                {
                    // Load view model with additional display info wrt parent course
                    Course course = new Models.Course();
                    course = db.Courses.Find(viewModel.CourseId);
                    viewModel.CourseModules = new List<ModuleListData>();
                    foreach (var mod in course.Modules)
                    {
                        ModuleListData courseModule = new ModuleListData();
                        courseModule.Id = mod.Id;
                        courseModule.Name = mod.Name;
                        viewModel.CourseModules.Add(courseModule);
                    }
                    viewModel.CourseName = course.Name;

                    if (viewModel.PostOperation == "Update")
                    {
                        // Load view model with additional display info wrt activities
                        viewModel.ModuleActivities = new List<ActivityListData>();
                        foreach (var act in module.Activities)
                        {
                            ActivityListData moduleActivity = new ActivityListData();
                            moduleActivity.Id = act.Id;
                            moduleActivity.Name = act.Name;
                            viewModel.ModuleActivities.Add(moduleActivity);
                        }
                    }
                }
 
                switch (viewModel.PostNavigation)
                {
                    case "Save":
                        return View(viewModel);
                        //break;
                    case "SaveRet":
                        return RedirectToAction("Manage", "Courses", new { id = viewModel.CourseId });
                        //break;
                    case "SaveAct":
                        return RedirectToAction("Manage", "Activities", new { id = viewModel.ActivityId, moduleId = viewModel.Id, courseId = viewModel.CourseId });
                        //break;
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
            if (((id == null) && (deleteType == "Single")) || (courseId == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModuleDeleteViewModel viewModel = new ModuleDeleteViewModel();
            if (deleteType == "Single")
            {
                Module module = db.Modules.Find(id);
                if (module == null)
                {
                    return HttpNotFound();
                }

                // Init specifically required fields
                viewModel.Id = module.Id;
                viewModel.Name = module.Name;
                viewModel.Description = module.Description;
                viewModel.StartDate = module.StartDate;
                viewModel.EndDate = module.EndDate;
            }
            if (deleteType == "All")
            {
                // Init specifically required fields
                Course course = new Models.Course();
                course = db.Courses.Find(courseId);
                viewModel.CourseModules = new List<ModuleListData>();
                foreach (var mod in course.Modules)
                {
                    ModuleListData courseModule = new ModuleListData();
                    courseModule.Id = mod.Id;
                    courseModule.Name = mod.Name;
                    viewModel.CourseModules.Add(courseModule);
                }
            }
            viewModel.DeleteType = deleteType;
            viewModel.CourseId = (int) courseId;
            return View(viewModel);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id, int? courseId, string deleteType)
        {
            if (deleteType == "Single")
            {
                Module module = db.Modules.Find(id);
                var moduleName = module.Name;
                db.Modules.Remove(module);
                db.SaveChanges();
                return RedirectToAction("Manage", "Modules", new { courseId = courseId, viewMessage = "The " + @moduleName + " module was removed" });
            }
            else // deleteType == "All"
            {
                Course course = new Models.Course();
                course = db.Courses.Find(courseId);
                var courseModuleIds = new List<int>();
                foreach (var mod in course.Modules)
                {
                    courseModuleIds.Add(mod.Id);
                }
                foreach (var modId in courseModuleIds)
                {
                    Module module = db.Modules.Find(modId);
                    db.Modules.Remove(module);
                    db.SaveChanges();
                }
                return RedirectToAction("Manage", "Modules", new { courseId = courseId, viewMessage = "All modules removed" });
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
