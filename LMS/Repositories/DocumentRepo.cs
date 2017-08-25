using LMS.Models;
using LMS.ViewModels;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace LMS.Repositories
{
    public static class DocumentRepo
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        private static List<DocumentListData> GetDocumentListViewModel(List<Document> documents)
        {
            var documentList = new List<DocumentListData>();
            foreach (var document in documents)
            {
                documentList.Add(new DocumentListData
                {
                    Id = document.Id,
                    Name = document.Name
                });
            }
            return documentList;
        }

        public static List<DocumentListData> GetCourseDocumentListViewModel(int courseId)
        {
            return GetDocumentListViewModel(db.Documents.Where(d => d.CourseId == courseId).ToList());
        }

        public static List<DocumentListData> GetModuleDocumentListViewModel(int moduleId)
        {
            return GetDocumentListViewModel(db.Documents.Where(d => d.ModuleId == moduleId).ToList());
        }

        public static List<DocumentListData> GetActivityDocumentListViewModel(int activityId)
        {
            return GetDocumentListViewModel(db.Documents.Where(d => d.ActivityId == activityId).ToList());
        }

        public static List<DocumentListData> GetUserDocumentListViewModel(string userId)
        {
            return GetDocumentListViewModel(db.Documents.Where(d => d.UserId == userId).ToList());
        }

        public static string DocumentNameIsUnique(DocumentViewModel documentViewModel)
        {
            if (documentViewModel.SiblingDocuments == null)
            {
                return null;
            }
            foreach (var doc in documentViewModel.SiblingDocuments)
                if (string.Equals(doc.Name, documentViewModel.Name, StringComparison.InvariantCultureIgnoreCase) && doc.Id != documentViewModel.Id)
                    return "Ogiltigt dokumentnamn. Det existerar redan ett dokument med samma namn";
            return null;
        }

        /// <summary>
        /// Create the viewModel to be displayed. If existing document Id is used to fetch db-record. All other params are ignored
        /// If Id is null or 0 a model for a new document is prepared. In this case one, and only one, of userId
        /// courseId, moduleId and activityId should be set --> The owner of the document
        /// </summary>
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
                documentViewModel.FileName = dbDocument.FileName;
                documentViewModel.Description = dbDocument.Description;
                documentViewModel.DocumentType = dbDocument.DokumentType;
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
                //documentViewModel.FileName = "Ingen fil har valts";
            }
            documentViewModel.PostMessage = postMessage;

            if (!string.IsNullOrEmpty(documentViewModel.UserId))
            {
                documentViewModel.SiblingDocuments = GetUserDocumentListViewModel(documentViewModel.UserId);
                documentViewModel.UserName = "document.username not handled yet";
            }
            else if (documentViewModel.ActivityId > 0)
            {
                documentViewModel.SiblingDocuments = GetActivityDocumentListViewModel((int)documentViewModel.ActivityId);
                documentViewModel.CourseName = dbRepo.ActivityId2courseName(documentViewModel.ActivityId);
                documentViewModel.ModuleName = dbRepo.ActivityId2moduleName(documentViewModel.ActivityId);
                documentViewModel.ActivityName = dbRepo.ActivityId2activityName(documentViewModel.ActivityId);
                documentViewModel._CourseId = dbRepo.ActivityId2courseId(documentViewModel.ActivityId);
                documentViewModel._ModuleId = dbRepo.ActivityId2moduleId(documentViewModel.ActivityId);
                documentViewModel._ActivityId = documentViewModel.ActivityId;
            }
            else if (documentViewModel.ModuleId > 0)
            {
                documentViewModel.SiblingDocuments = GetModuleDocumentListViewModel((int)documentViewModel.ModuleId);
                documentViewModel.CourseName = dbRepo.ModuleId2courseName(documentViewModel.ModuleId);
                documentViewModel.ModuleName = dbRepo.ModuleId2moduleName(documentViewModel.ModuleId);
                documentViewModel._CourseId = dbRepo.ModuleId2courseId(documentViewModel.ModuleId);
                documentViewModel._ModuleId = documentViewModel.ActivityId;

            }
            else if (documentViewModel.CourseId > 0)
            {
                documentViewModel.SiblingDocuments = GetCourseDocumentListViewModel((int)documentViewModel.CourseId);
                documentViewModel.CourseName = dbRepo.CourseId2courseName(documentViewModel.CourseId);
                documentViewModel._CourseId = documentViewModel.CourseId;
            }
            //else !!! HANDLE ERROR !!!

            documentViewModel.LongCourseName = string.Join(", ", new string[] { documentViewModel.CourseName, documentViewModel.ModuleName, documentViewModel.ActivityName });

            return documentViewModel;
        }

        /// <summary>
        /// Save the viewModel to the database
        /// </summary>
        /// 
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
                dbDocument.CourseId = documentViewModel.CourseId;
                dbDocument.ModuleId = documentViewModel.ModuleId;
                dbDocument.ActivityId = documentViewModel.ActivityId;
                dbDocument.UserId = documentViewModel.UserId;
            }
            dbDocument.Name = documentViewModel.Name;
            dbDocument.Description = documentViewModel.Description;
            dbDocument.DokumentType = documentViewModel.DocumentType;
            dbDocument.UploadDate = DateTime.Now; //documentViewModel.UploadDate;

            dbDocument.FileName = documentViewModel.FileName;
            dbDocument.UploadedFileName = documentViewModel.UploadedFileName;
            dbDocument.Format = documentViewModel.Format;
            
            if (documentViewModel.Id > 0)
            {
                Document oldDocument = db.Documents.Find(documentViewModel.Id);
                NotificationRepo.AddChangedDocumentNote(oldDocument, dbDocument);
                db.Entry(dbDocument).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                NotificationRepo.AddNewDocumentNote(dbDocument);
                db.Documents.Add(dbDocument);
                db.SaveChanges();
            }
            return dbDocument.Id;
        }


        // RETREIVE a list of documents (id + name + etc) for a course / module / activity
        public static List<DocumentListData> RetrieveCourseDocumentList(int? courseId, int? moduleId, int? activityId)
        {
            if (activityId > 0)
            {
                return GetActivityDocumentListViewModel((int)activityId);
            }
            else if (moduleId > 0)
            {
                return GetModuleDocumentListViewModel((int)moduleId);
            }
            else if (courseId > 0)
            {
                return GetCourseDocumentListViewModel((int)courseId);
            }
            else
                return null; //else !!! HANDLE ERROR !!!


            //var documents = db.Documents.Where(d => d.CourseId == courseId).Where(d => d.ModuleId == moduleId).Where(d => d.ActivityId == activityId).ToList();
            //var documentList = new List<DocumentListData>();
            //foreach (var doc in documents)
            //{
            //    DocumentListData courseDocument = new DocumentListData();
            //    courseDocument.Id = doc.Id;
            //    courseDocument.Name = doc.Name;
            //    courseDocument.Description = doc.Description;
            //    courseDocument.DocumentType = doc.DokumentType;
            //    courseDocument.UploadDate = doc.UploadDate;
            //    documentList.Add(courseDocument);
            //}           
            //return documentList;
        }


        // RETREIVE number of documents for a course / module / activity
        public static int RetrieveNoOfDocuments(int? courseId, int? moduleId, int? activityId)
        {
            var documents = db.Documents.Where(d => d.CourseId == courseId).Where(d => d.ModuleId == moduleId).Where(d => d.ActivityId == activityId).ToList();
            if (documents == null)
                return 0;
            return documents.Count;
        }


    }
}
