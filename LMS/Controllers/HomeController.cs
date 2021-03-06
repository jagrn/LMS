﻿using LMS.Models;
using LMS.Repositories;
using LMS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace LMS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Courses");
            //return View();
        }

        public ActionResult Upload()
        {
            DocumentViewModel doc;

            HttpPostedFileBase fileetxt;

            if (Request.Files.Count>0)
            {
                fileetxt = Request.Files[0];
            }

            foreach (string upload in Request.Files)
            {
                if (Request.Files[upload].FileName != "") 
               {
                    Guid g = Guid.NewGuid();
                    string path = AppDomain.CurrentDomain.BaseDirectory + "uploads\\";
                    string filename = Path.GetFileName(Request.Files[upload].FileName);
                    doc = DocumentRepo.GetDocumentViewModel(null, null, null, 1, null, null);
                    doc.Name = filename;
                    doc.FileName = filename;
                    doc.Format = Path.GetExtension(filename);
                    filename = Guid.NewGuid().ToString() + doc.Format;
                    Request.Files[upload].SaveAs(Path.Combine(path, filename));
                   // var s = Request.Files[upload];
                    DocumentRepo.PostDocumentViewModel(doc);
                    var s = Path.Combine(path, filename);
                    System.Diagnostics.Process.Start(s);
                    //System.Diagnostics.Process.Start("@" + Path.Combine(path, filename));
                }
            }
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}