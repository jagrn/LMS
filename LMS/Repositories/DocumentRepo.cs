using LMS.Models;
using LMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Repositories
{
    public class DocumentRepo
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        // QUERY whether a certain document (id) exists
        public static bool IsDocumentPresent(int? id)
        {
            return ((id != null) && (db.Documents.Find(id) != null));
        }


        public DocumentViewModel GetDocumentViewModel(int? documentId, int? courseId, int? moduleId, int? activityId, string userId)
        {
            var documentViewModel = new DocumentViewModel();
            if (documentId > 0)
            {
                Document document = db.Documents.Find(documentId);
                if (document == null)
                {
                    /// handle error
                }
                documentViewModel.Id = document.Id;
                documentViewModel.Name = document.Name;
                documentViewModel.Description = document.Description;
                documentViewModel.Format = document.Format;
                documentViewModel.UploadDate = document.UploadDate;
                documentViewModel.CourseId = document.CourseId;
                documentViewModel.ModuleId = document.ModuleId;
                documentViewModel.ActivityId = document.ActivityId;
                documentViewModel.UserId = document.UserId;
            }

            List<Document> documents = new List<Document>();
            if (activityId > 0)
                documents = db.Documents.Where(d => d.ActivityId == activityId).ToList();
            else if (moduleId > 0)
                documents = db.Documents.Where(d => d.ModuleId == moduleId).ToList();
            else if (courseId > 0)
                documents = db.Documents.Where(d => d.CourseId == courseId).ToList();
            else if (!string.IsNullOrWhiteSpace(userId))
                documents = db.Documents.Where(d => d.UserId == userId).ToList();
            //else !!! HANDLE ERROR !!!
            foreach (var doc in documents)
            {
                DocumentListData documentListData = new DocumentListData();
                documentListData.Id = doc.Id;
                documentListData.Name = doc.Name;
                documentViewModel.SiblingDocuments.Add(documentListData);
            }
            return documentViewModel;
        }
    }
}