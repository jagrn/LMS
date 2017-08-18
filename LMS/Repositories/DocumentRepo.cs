using LMS.Models;
using LMS.ViewModels;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Repositories
{
    public static class DocumentRepo
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        // QUERY whether a certain document (id) exists
        public static bool IsDocumentPresent(int? id)
        {
            return ((id != null) && (db.Documents.Find(id) != null));
        }

        public static string IsDocumentNameValid(DocumentViewModel documentViewModel)
        {
            if (documentViewModel.SiblingDocuments == null)
            {
                return null;
            }
            foreach (var doc in documentViewModel.SiblingDocuments)
                if (string.Equals(doc.Name, documentViewModel.Name, StringComparison.OrdinalIgnoreCase) && doc.Id != documentViewModel.Id)
                    return "Ogiltigt dokumentnamn. Det existerar redan ett dokument med samma namn";
            return null;
        }


        public static DocumentViewModel GetDocumentViewModel(int? Id, int? courseId, int? moduleId, int? activityId, string userId, string postMessage)
        {
            var documentViewModel = new DocumentViewModel();
            if (Id > 0)
            {
                Document dbDocument = db.Documents.Find(Id);
                if (dbDocument == null)
                {
                    /// handle error
                }
                documentViewModel.Id = dbDocument.Id;
                documentViewModel.Name = dbDocument.Name;
                documentViewModel.Description = dbDocument.Description;
                documentViewModel.Format = dbDocument.Format;
                documentViewModel.UploadDate = dbDocument.UploadDate;
                documentViewModel.CourseId = dbDocument.CourseId;
                documentViewModel.ModuleId = dbDocument.ModuleId;
                documentViewModel.ActivityId = dbDocument.ActivityId;
                documentViewModel.UserId = dbDocument.UserId;
            }
            else
            {
                documentViewModel.CourseId = courseId;
                documentViewModel.ModuleId = moduleId;
                documentViewModel.ActivityId = activityId;
                documentViewModel.UserId = userId;
                documentViewModel.UploadDate = DateTime.Now;
            }
            documentViewModel.PostMessage = postMessage;

            List<Document> documents = new List<Document>();
            if (!string.IsNullOrEmpty(userId))
                documents = db.Documents.Where(d => d.UserId == userId).ToList();
            if (activityId > 0)
                documents = db.Documents.Where(d => d.ActivityId == activityId).ToList();
            else if (moduleId > 0)
                documents = db.Documents.Where(d => d.ModuleId == moduleId).ToList();
            else if (courseId > 0)
                documents = db.Documents.Where(d => d.CourseId == courseId).ToList();
            //else !!! HANDLE ERROR !!!
            documentViewModel.SiblingDocuments = new List<DocumentListData>();
            foreach (var doc in documents)
            {
                DocumentListData documentListData = new DocumentListData();
                documentListData.Id = doc.Id;
                documentListData.Name = doc.Name;
                documentViewModel.SiblingDocuments.Add(documentListData);
            }
            return documentViewModel;
        }

        public static int PostDocumentViewModel(DocumentViewModel documentViewModel)
        {
            Document dbDocument;
            if (documentViewModel.Id > 0)
            {
                dbDocument = db.Documents.Find(documentViewModel.Id);
                if (dbDocument == null)
                {
                    /// handle error
                }
            }
            else
            {
                dbDocument = new Document();
            }
            //documentViewModel.Id = document.Id;
            dbDocument.Name = documentViewModel.Name;
            dbDocument.Description = documentViewModel.Description;
            dbDocument.Format = documentViewModel.Format;
            dbDocument.UploadDate = documentViewModel.UploadDate;
            dbDocument.CourseId = documentViewModel.CourseId;
            dbDocument.ModuleId = documentViewModel.ModuleId;
            dbDocument.ActivityId = documentViewModel.ActivityId;
            dbDocument.UserId = documentViewModel.UserId;

            if (documentViewModel.Id > 0)
            {
                db.Entry(dbDocument).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                db.Documents.Add(dbDocument);
                db.SaveChanges();
            }
            return dbDocument.Id;
        }

    }
}