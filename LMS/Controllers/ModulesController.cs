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

        //// GET: Modules/Manage/5
        //public ActionResult Manage(int? id, int? courseId)
        //{
        //    Module module = new Module();
        //    if (id == null)
        //    {
        //        if (courseId == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        module.CourseId = (int)courseId; 
        //    }
        //    else
        //    {
        //        module = db.Modules.Find(id);
        //        if (module == null)
        //        {
        //            return HttpNotFound();
        //        }
        //    }
        //    return View(module);
        //}

        // GET: Modules/Manage/5
        public ActionResult Manage(int? id, int? courseId)
        {
            ManageModuleViewModel viewModule = new ManageModuleViewModel();
            if (id == null)
            {
                if (courseId == null)
                {
                    return HttpNotFound();
                }
                viewModule.CourseId = (int)courseId;
            }
            else
            {
                Module module = new Module();
                module = db.Modules.Find(id);
                if (module == null)
                {
                    return HttpNotFound();
                }

                viewModule.Id = module.Id;
                viewModule.Name = module.Name;
                viewModule.Description = module.Description;
                viewModule.StartDate = module.StartDate;
                viewModule.EndDate = module.EndDate;
                viewModule.CourseId = module.CourseId;
                viewModule.Activities = module.Activities;
                //viewModule.ProceedPath = "Test";
            }
            return View(viewModule);
        }


        // POST: Modules/Manage/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId,ProceedPath")] ManageModuleViewModel viewModule)
        {
            if (viewModule.Id == 0)
            {
                // Create
                if (ModelState.IsValid)
                {
                    Module module = new Module();
                    module.Name = viewModule.Name;
                    module.Description = viewModule.Description;
                    module.StartDate = viewModule.StartDate;
                    module.EndDate = viewModule.EndDate;
                    module.CourseId = viewModule.CourseId;
                    module.Activities = viewModule.Activities;

                    db.Modules.Add(module);
                    db.SaveChanges();
                    return RedirectToAction(viewModule.ProceedPath);
                }
            }
            else
            {
                // Edit
                if (ModelState.IsValid)
                {
                    Module module = new Module();
                    module.Name = viewModule.Name;
                    module.Description = viewModule.Description;
                    module.StartDate = viewModule.StartDate;
                    module.EndDate = viewModule.EndDate;
                    module.CourseId = viewModule.CourseId;
                    module.Activities = viewModule.Activities;

                    db.Entry(module).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction(viewModule.ProceedPath);
                }

            }
            return View(viewModule);
        }

        //// POST: Modules/Manage/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Manage([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId")] Module module, int? courseId)
        //{
        //    if (module.Id == 0)
        //    {
        //        // Create
        //        if (ModelState.IsValid)
        //        {
        //            db.Modules.Add(module);
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    else
        //    {
        //        // Edit
        //        if (ModelState.IsValid)
        //        {
        //            db.Entry(module).State = EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }

        //    }
        //    return View(module);
        //}


        

        // GET: Modules/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Module module = db.Modules.Find(id);
            db.Modules.Remove(module);
            db.SaveChanges();
            return RedirectToAction("Index");
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
