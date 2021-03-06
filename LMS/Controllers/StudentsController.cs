﻿using LMS.Models;
using LMS.Repositories;
using LMS.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace LMS.Controllers
{
    public struct PeriodData
    {
        public int Year;
        public int Week;
        public DateTime Start;
        public DateTime End;
    }

    public class StudentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
   

        // GET: Students/MyPage
        public ActionResult MyPage(int? courseId, int? moduleId, int? activityId, string studentId, int? schemeYear, int? schemeWeek, int? schemeMoveWeek, bool fromMyPage)
        {
            //Get currentUser in order to get courseId
            ApplicationUser currentUser = db.Users.Find(User.Identity.GetUserId());

            courseId = currentUser.CourseId;
  
            studentId = currentUser.Id;

           
            if ((courseId == 0) || (courseId == null) || (studentId == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StudentViewModel viewModel = new StudentViewModel();
            viewModel.CourseId = (int)courseId;
            viewModel.CourseDescription = CourseRepo.RetrieveCourseDescription((int)courseId);
            viewModel.StudentId = studentId;
            viewModel.StudentName = StudentRepo.GetStudentName(studentId);
            //viewModel.StudentId = StudentRepo.GetFakeStudentId();

            bool showModuleDetails = false;
            if ((moduleId != null) && (moduleId != 0))
            {
                viewModel.ModuleId = (int)moduleId;
                // Load a selected module
                viewModel.Module = new Models.Module();
                var singleModule = ModuleRepo.RetrieveModule(courseId, moduleId);
                if (singleModule.repoResult == ModuleRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                viewModel.Module.Name = singleModule.module.Name;
                viewModel.Module.Description = singleModule.module.Description;
                viewModel.Module.StartDate = singleModule.module.StartDate;
                viewModel.Module.EndDate = singleModule.module.EndDate;

                viewModel.DocumentList = DocumentRepo.RetrieveCourseDocumentList((int)courseId, (int)moduleId, null);

                showModuleDetails = true;
            }
            else
            {
                viewModel.ModuleId = 0;
            }

            bool showActivityDetails = false;
            viewModel.Activity = new Models.Activity();
            if ((activityId != null) && (activityId != 0))
            {
                viewModel.ActivityId = (int)activityId;
                // Load a selected activity
                
                var singleActvivity = ActivityRepo.RetrieveActivity(moduleId, activityId);
                if (singleActvivity.repoResult == ActivityRepoResult.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                viewModel.Activity.Name = singleActvivity.activity.Name;
                viewModel.Activity.Description = singleActvivity.activity.Description;
                viewModel.Activity.StartDate = singleActvivity.activity.StartDate;
                viewModel.Activity.EndDate = singleActvivity.activity.EndDate;
                viewModel.Activity.ActivityType = singleActvivity.activity.ActivityType;
                showActivityDetails = true;

                viewModel.DocumentList = DocumentRepo.RetrieveCourseDocumentList((int)courseId, (int)moduleId, activityId);

                showModuleDetails = false;
           }
            else
            {
               
                viewModel.ActivityId = 0;
            }

            // Load view model with additional display info wrt course modules
            var courseModuleList = ModuleRepo.RetrieveCourseModuleList(courseId);
            if (courseModuleList.repoResult == ModuleRepoResult.NotFound)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            viewModel.CourseModules = courseModuleList.moduleList;
            viewModel.CourseName = CourseRepo.RetrieveCourseName(courseId);

            // Load view model with additional display info wrt module activities
            var moduleActivityList = ActivityRepo.RetrieveModuleActivityList(moduleId);
            if (moduleActivityList.repoResult == ActivityRepoResult.NotFound)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            viewModel.ModuleActivities = moduleActivityList.activityList;
            //var docs = db.Documents.Where(d => d.CourseId == courseId && d.ModuleId == moduleId && d.ActivityId == activityId);
            //docs = docs.OrderBy(d => d.Name);

            //viewModel.Activity.Documents = docs.ToList();


            viewModel.Notifications = StudentRepo.RetreiveNotesForStudent(viewModel.StudentId);
            viewModel.NoOfNotifications = viewModel.Notifications.Count;

            if (schemeYear == null)
            {
                schemeYear = 2000;
            }
            viewModel.SchemeYear = (int) schemeYear;
            if (schemeWeek == null)
            {
                schemeWeek = 1;
            }
            viewModel.SchemeWeek = (int) schemeWeek;
            viewModel.SchemeMoveWeek = schemeMoveWeek;

            if (fromMyPage)
            {
                if (showModuleDetails)
                {
                    var start = viewModel.Module.StartDate;
                    viewModel.SchemeYear = start.Year;
                    viewModel.SchemeWeek = GetWeekFromDate(start);
                    viewModel.SchemeMoveWeek = null;
                }

                if (showActivityDetails)
                {
                    var start = viewModel.Activity.StartDate;
                    viewModel.SchemeYear = start.Year;
                    viewModel.SchemeWeek = GetWeekFromDate(start);
                    viewModel.SchemeMoveWeek = null;
                }
            }




            return View(viewModel);
        }

        public ActionResult CourseModuleMenu()
        {
            ApplicationUser currentUser = db.Users.Find(User.Identity.GetUserId());

            int courseId = (int)currentUser.CourseId;
            TestController tc = new TestController();

            return tc.GetCourseModuleMenu(courseId);
        }

        public ActionResult ReadNote(int? moduleId, int? activityId, int? schemeYear, int? schemeWeek, int? schemeMoveWeek, int? noteId)
        {
            //Get currentUser in order to studentId
            ApplicationUser currentUser = db.Users.Find(User.Identity.GetUserId());
            var studentId = currentUser.Id;

            if ((noteId != null) && (noteId != 0))
            {
                StudentRepo.UpdateNoteForStudent(studentId, (int) noteId);
            }
            return RedirectToAction("MyPage", new { moduleId = moduleId, activityId = activityId, schemeYear = schemeYear, schemeWeek = schemeWeek, schemeMoveWeek = schemeMoveWeek, fromMyPage = true });
        }

        public ActionResult ReadAllNotes(int? moduleId, int? activityId, int? schemeYear, int? schemeWeek, int? schemeMoveWeek)
        {
            //Get currentUser in order to studentId
            ApplicationUser currentUser = db.Users.Find(User.Identity.GetUserId());
            var studentId = currentUser.Id;
            
            StudentRepo.UpdateAllNotesForStudent(studentId);
            
            return RedirectToAction("MyPage", new { moduleId = moduleId, activityId = activityId, schemeYear = schemeYear, schemeWeek = schemeWeek, schemeMoveWeek = schemeMoveWeek, fromMyPage = true });
        }


        [Authorize(Roles = "Student")]
        public ActionResult ListStudents(string sortOrder)
        {

            IQueryable<StudentListViewModel> query;
            List<StudentListViewModel> resultList;

            //Get currentUser in order to get courseId
            ApplicationUser currentUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.CourseName = Repositories.CourseRepo.RetrieveCourseName(currentUser.CourseId);
            
            if (currentUser.CourseId != null)
            {
                query = from u in db.Users
                        where u.CourseId == currentUser.CourseId
                        select new StudentListViewModel()
                        {
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email

                        };

                //Toggle sortorder
                ViewBag.SortLastName = sortOrder == "LastName_desc" ?  "LastName" : "LastName_desc";
                ViewBag.SortFirstName = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
                ViewBag.SortEmail = sortOrder == "Email" ? "Email_desc" : "Email";

                switch (sortOrder)
                {
                    case "LastName":
                        query = query.OrderBy(s => s.LastName);
                        break;
                    case "LastName_desc":
                        query = query.OrderByDescending(s => s.LastName);
                        break;
                    case "FirstName":
                        query = query.OrderBy(s => s.FirstName);
                        break;
                    case "FirstName_desc":
                        query = query.OrderByDescending(s => s.FirstName);
                        break;
                    case "Email":
                        query = query.OrderBy(s => s.Email);
                        break;
                    case "Email_desc":
                        query = query.OrderByDescending(s => s.Email);
                        break;
                     default:
                        query = query.OrderBy(s => s.LastName);
                        break;
                }

   
                
                resultList = query.ToList();
                ViewBag.Count = resultList.Count();
                return View(resultList.ToList());

            }
            else
            {
                ViewBag.Count = 0;
                return View(new List<StudentListViewModel>().DefaultIfEmpty());
            }
          
            

        }

        public ActionResult Test()
        {
            return View();
        }
        private DateTime GetMonday(int year, int week)
        {
            /// Converts a week number to corresponding monday date.
            /// Note: Week 1 of a year may start in the previous year.
            /// ISO 8601 week 1 is the week that contains the first Thursday that year.
            /// If December 28 is a Monday, December 31 is a Thursday and week 1 starts January 4.
            /// If December 28 is a Sunday, December 31 is a Wednesday and week 1 starts December 29 previous year.
            var dec28 = new DateTime (year - 1, 12, 28);
            var dec28DayOffset = ((int)dec28.DayOfWeek + 6) % 7;    // Monday = 0, Tuesday = 1, etc
            return dec28.AddDays(7 * week - dec28DayOffset);
        }

        private DateTime GetThisMonday()
        {
            var today = new DateTime();
            today = DateTime.Now;
            var dayOffset = ((int)(today.DayOfWeek + 6)) % 7;       // Monday = 0, Tuesday = 1, etc
            return today.AddDays(-dayOffset);
        }
  
        private int GetWeek(DateTime monday)
        {
            /// Converts a monday date to a corresponding week number.
            /// ISO 8601 week 1 is the week that contains the first Thursday that year.
            var thursday = monday.AddDays(3);
            return (thursday.DayOfYear - 1) / 7 + 1;
        }

        private int GetWeekFromDate(DateTime date)
        {
            var dayOffset = ((int)(date.DayOfWeek + 6)) % 7;       // Monday = 0, Tuesday = 1, etc
            var monday = date.AddDays(-dayOffset);
            var week = GetWeek(monday);
            return week;
        }

        private PeriodData GetWeekPeriod(int year, int week, int moveWeek)
        {
            // Get next Monday reference based on old year/week and moveWeek
            DateTime nextMonday = new DateTime();
            if (moveWeek == 0)
            {
                nextMonday = GetThisMonday();
            }
            else
            {
                nextMonday = GetMonday(year, week);
                nextMonday = nextMonday.AddDays(7 * moveWeek);
            }

            PeriodData periodData = new PeriodData();
            var monday = nextMonday.ToShortDateString() + " 08:30:00";
            periodData.Start = DateTime.Parse(monday);
            var friday = nextMonday.AddDays(4).ToShortDateString() + " 16:30:00";
            periodData.End = DateTime.Parse(friday);
            periodData.Year = nextMonday.Year;
            periodData.Week = GetWeek(nextMonday);
            return periodData;
        }

        private int GetIndex(PeriodActivityListData activityData, DateTime startOfWeek, bool fullDay)
        {
            // Returns a session index reflecting the scheme position (within a week)
            // AM sessions are indexed 0..4
            // PM sessions are indexed 5..9
            // Fullday sessions are indexed 10.14

            var index = ((int)(activityData.StartDate.DayOfWeek + 6)) % 7;       // Day offset: Monday = 0, Tuesday = 1, etc
            var indexStart = startOfWeek.AddDays(index);
            if (fullDay)
            {
                index = index + 10;         // Full day sessions
            }
            else
            {
                if (activityData.StartDate != indexStart)
                {                  
                    index = index + 5;      // Afternoon session
                }
            }
            return index;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }



        // GET: Students/Scheme
        public ActionResult Scheme(int? courseId, int? year, int? week, int? moveWeek, int myPageModuleId, int myPageActivityId, string myPageStudentId)
        {
            SchemeViewModel viewModel = new SchemeViewModel();
            if ((courseId == null) || (courseId == 0))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                //year = 2017;
                //week = 1;
                //moveWeek = 0;
            }

            viewModel.courseId = (int) courseId;           
            viewModel.MyPageModuleId = myPageModuleId;
            viewModel.MyPageActivityId = myPageActivityId;
            viewModel.MyPageStudentId = myPageStudentId;

            PeriodData periodData; //= GetWeekPeriod((int)year, (int)week, (int)moveWeek);
            if (moveWeek != null)
            {
                periodData = GetWeekPeriod((int)year, (int)week, (int)moveWeek);
            }
            else
            {
                periodData = GetWeekPeriod((int)year, (int)week-1, 1);
            }

            viewModel.Year = periodData.Year;
            viewModel.Week = periodData.Week;
            viewModel.Monday = periodData.Start;
            viewModel.Period = periodData.Start.Date + " -- " + periodData.End.Date;
            
            PeriodActivityList activityList = ActivityRepo.RetrievePeriodActivities(courseId, periodData.Start, periodData.End);
            if (activityList.repoResult == ActivityRepoResult.NotFound)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            viewModel.WeekActivities = new List<SchemeActivity>();
            for (int index = 0; index < 10; index++)
            {
                SchemeActivity schemeAct = new SchemeActivity();
                schemeAct.ActivityType = -1;
                schemeAct.NameText = "";
                schemeAct.TypeText = "";
                schemeAct.ModuleNameText = "";
                schemeAct.ActivityId = 1;
                viewModel.WeekActivities.Add(schemeAct);
            }

            foreach (var act in activityList.periodActivityList)
            {
                SchemeActivity schemeAct = new SchemeActivity();
                schemeAct.ActivityType = (int) act.ActivityType;
                schemeAct.NameText = act.Name;

                string type = act.ActivityType.ToString();

                schemeAct.TypeText = type;
                schemeAct.ModuleNameText = act.ModuleName;
                schemeAct.ActivityId = 1;
                
                var index = GetIndex(act, activityList.startOfWeek, (act.ActivityPeriod == Models.ActivityPeriod.FullDay));
                if (index < 10)
                {
                    viewModel.WeekActivities.RemoveAt(index);
                    viewModel.WeekActivities.Insert(index, schemeAct);
                }
                else
                {
                    string typeText = schemeAct.TypeText;
                    schemeAct.TypeText = typeText + " (del 1)";
                    viewModel.WeekActivities.RemoveAt(index-10);
                    viewModel.WeekActivities.Insert(index-10, schemeAct);
                    schemeAct.TypeText = typeText + " (del 2)";
                    viewModel.WeekActivities.RemoveAt(index - 5);
                    viewModel.WeekActivities.Insert(index - 5, schemeAct);
                }
            }

            viewModel.Notifications = NotificationRepo.RetrieveCourseNotes((int) courseId);

            return PartialView(viewModel);
        }

        // GET: Students/Scheme
        //public ActionResult Scheme2(int? courseId, int? year, int? week, int? moveWeek)
        //{
        //    SchemeViewModel viewModel = new SchemeViewModel();
        //    if (courseId == null)
        //    {
        //        year = 2017;
        //        week = 1;
        //        moveWeek = 0;
        //    }

        //    var periodData = GetWeekPeriod((int)year, (int)week, (int)moveWeek);
        //    viewModel.Year = periodData.Year;
        //    viewModel.Week = periodData.Week;
        //    viewModel.Monday = periodData.Start;
        //    viewModel.Period = periodData.Start.Date + " -- " + periodData.End.Date;
        //    viewModel.WeekActivities = new List<SchemeActivity>();

        //    for (int index = 0; index < 10; index++)
        //    {
        //        SchemeActivity schemeAct = new SchemeActivity();
        //        if ((index == 0) || (index == 5) || (index == 6) || (index == 7))
        //        {
        //            schemeAct.ActivityType = -1;
        //            schemeAct.NameText = "";
        //            schemeAct.TypeText = "";
        //            schemeAct.ActivityId = 1;
        //        }

        //        if (index == 2)
        //        {
        //            schemeAct.ActivityType = 0;
        //            schemeAct.NameText = "Java introduction";
        //            schemeAct.TypeText = "Föreläsning";
        //            schemeAct.ActivityId = 1;
        //        }

        //        if ((index == 3) || (index == 4))
        //        {
        //            schemeAct.ActivityType = 1;
        //            if (index == 3)
        //                schemeAct.NameText = "Become a Java coder";
        //            else
        //                schemeAct.NameText = "Advanced techniques in Java";
        //            schemeAct.TypeText = "Datorbaserad";
        //            schemeAct.ActivityId = 1;
        //        }

        //        if ((index == 1) || (index > 7))
        //        {
        //            schemeAct.ActivityType = 3;
        //            if (index == 1)
        //                schemeAct.NameText = "Java övning 1";
        //            else
        //                schemeAct.NameText = "Java övning 2";
        //            schemeAct.TypeText = "Övning";
        //            schemeAct.ActivityId = 1;
        //        }
        //        viewModel.WeekActivities.Add(schemeAct);
        //    }




        //    return View(viewModel);
        //}
    }
}