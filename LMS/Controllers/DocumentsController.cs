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
using System.IO;

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
        public ActionResult Manage(int? id, int? activityId, int? moduleId, int? courseId, string userId, string viewMessage)
        {
            DocumentViewModel documentViewModel = DocumentRepo.GetDocumentViewModel(id, courseId, moduleId, activityId, userId, viewMessage);

            documentViewModel.SiblingDocuments = DocumentRepo.RetrieveCourseDocumentList(courseId, moduleId, activityId);
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
            documentViewModel.SiblingDocuments = DocumentRepo.RetrieveCourseDocumentList(documentViewModel.CourseId, documentViewModel.ModuleId, documentViewModel.ActivityId);
            var validMess = DocumentRepo.DocumentNameIsUnique(documentViewModel);
            if (validMess != null)
            {
                documentViewModel.PostMessage = validMess;
                return View(documentViewModel);
            }
            // End of input validation

            string postMessage = "";
            if (documentViewModel.Id > 0)
                //documentViewModel.PostMessage = "Dokumentet " + documentViewModel.Name + " är uppdaterat";
                postMessage = "Dokumentet " + documentViewModel.Name + " är uppdaterat";
            else
                //documentViewModel.PostMessage = "Det nya dokumentet " + documentViewModel.Name + " är sparat";
                postMessage = "Det nya dokumentet " + documentViewModel.Name + " är sparat";

            if (Request.Files.Count>0)
            {
                documentViewModel.UploadedFile = Request.Files[0];
            }
            foreach (string upload in Request.Files)

            {
                if (Request.Files[upload].FileName != "")
                {
                    
                    Guid g = Guid.NewGuid();
                    string path = AppDomain.CurrentDomain.BaseDirectory + "uploads\\";
                    string fileName = Path.GetFileName(Request.Files[upload].FileName);
                    string fileExt = Path.GetExtension(fileName);

                    var doc = DocumentRepo.GetDocumentViewModel(null, null, null, 1, null, null);
                    doc.Name = fileName;
                    doc.FileName = fileName;
                    doc.Format = Path.GetExtension(fileName);
                    fileName = Guid.NewGuid().ToString() + doc.Format;
                    Request.Files[upload].SaveAs(Path.Combine(path, fileName));

                    DocumentRepo.PostDocumentViewModel(doc);
                    var s = Path.Combine(path, fileName);
                    System.Diagnostics.Process.Start(s);
                    //System.Diagnostics.Process.Start("@" + Path.Combine(path, filename));
                }
            }



            // SPARA SKER HÄR





            documentViewModel.Id = DocumentRepo.PostDocumentViewModel(documentViewModel);

            var document = db.Documents.Find(documentViewModel.Id);
            return RedirectToAction("Manage", new { id = documentViewModel.Id, courseId = document.CourseId, moduleId = document.ModuleId, activityId = document.ActivityId, viewMessage = postMessage });
        }
        //
        // end manage
        //

        // GET: Documents/Delete/5
        public ActionResult Delete(int? id, int? activityId, int? moduleId, int? courseId)
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

            DocumentDeleteViewModel viewModel = new DocumentDeleteViewModel();
            viewModel.Id = document.Id;
            viewModel.Name = document.Name;
            viewModel.DocumentType = document.DokumentType;
            viewModel.CourseId = courseId;
            viewModel.ModuleId = moduleId;
            viewModel.ActivityId = activityId;         
            return View(viewModel);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? courseId, int? moduleId, int? activityId )
        {
            Document document = db.Documents.Find(id);
            NotificationRepo.AddRemovedDocumentNote(document);
            db.Documents.Remove(document);
            db.SaveChanges();

            string message = "Dokumentet " + document.Name + " är borttaget";

            return RedirectToAction("Manage", "Documents", new { courseId = courseId, moduleId = moduleId, activityId = activityId, viewMessage = message });
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
