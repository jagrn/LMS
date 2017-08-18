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
    public class DocumentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Documents
        public ActionResult Index()
        {
            return View(db.Documents.ToList());
        }

        // GET: Documents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Documents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Format,UploadDate,CourseId,ModuleId,ActivityId,UserId")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Documents.Add(document);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(document);
        }

        // GET: Documents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Format,UploadDate,CourseId,ModuleId,ActivityId,UserId")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(document);
        }


        // GET: Activities/Manage/5
        public ActionResult Manage(int? id, int? activityId, int? moduleId, int? courseId, string userId, string getOperation, string viewMessage)
        {
            DocumentViewModel documentViewModel = DocumentRepo.GetDocumentViewModel(id, courseId, moduleId, activityId, userId, viewMessage);
            return View(documentViewModel);
        }

        // POST: Activities/Manage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Manage([Bind(Include = "Id,Name,Description,Format,UploadDate,CourseId,ModuleId,ActivityId,UserId,PostMessage,PostNavigation,PostOperation")] ActivityViewModel viewModel)
        public ActionResult Manage(DocumentViewModel documentViewModel)

        {
            if (!ModelState.IsValid)
            {
                return View(documentViewModel);
            }
            // Input validation
            var validMess = DocumentRepo.IsDocumentNameValid(documentViewModel);
            if (validMess != null)
            {
                documentViewModel.PostMessage = validMess;
                return View(documentViewModel);
            }
            // End of input validation

            if (documentViewModel.Id > 0)  //endast under test. Ska ändras till "Dokumentet är sparat" oavsett nytt eller ej
                documentViewModel.PostMessage = "Dokumentet " + documentViewModel.Name + " är uppdaterat (byt meddelande efter testperiod)";
            else
                documentViewModel.PostMessage = "Den nya dokumentet " + documentViewModel.Name + " är sparad";

            // SPARA SKER HÄR
            documentViewModel.Id = DocumentRepo.PostDocumentViewModel(documentViewModel);


            return RedirectToAction("Manage", new
            {
                id = documentViewModel.Id,
                moduleId = documentViewModel.ModuleId,
                courseId = documentViewModel.CourseId,
                userId = documentViewModel.UserId,
                getOperation = "Load",
                viewMessage = documentViewModel.PostMessage
            });
        }
        //
        // end manage
        //

        // GET: Documents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Document document = db.Documents.Find(id);
            db.Documents.Remove(document);
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
