using LMS.Repositories;
using LMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
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
        public ActionResult Scheme(int? courseId, int? year, int? week, int? moveWeek)
        {
            SchemeViewModel viewModel = new SchemeViewModel();
            if (courseId == null)
            {
                year = 2017;
                week = 1;
                moveWeek = 0;
            }

            var periodData = GetWeekPeriod((int)year, (int)week, (int)moveWeek);
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
                    viewModel.WeekActivities.RemoveAt(index-10);
                    viewModel.WeekActivities.Insert(index-10, schemeAct);
                    viewModel.WeekActivities.RemoveAt(index - 5);
                    viewModel.WeekActivities.Insert(index - 5, schemeAct);
                }
            }

            //for (int index = 0; index < 10; index++)
            //{
            //    SchemeActivity schemeAct = new SchemeActivity();
            //    if ((index == 0)|| (index == 5) || (index == 6) || (index == 7))
            //    {
            //        schemeAct.ActivityType = -1;
            //        schemeAct.NameText = "";
            //        schemeAct.TypeText = "";
            //        schemeAct.ActivityId = 1;
            //    }

            //    if (index == 2)
            //    {
            //        schemeAct.ActivityType = 0;
            //        schemeAct.NameText = "Java introduction";
            //        schemeAct.TypeText = "Föreläsning";
            //        schemeAct.ActivityId = 1;
            //    }

            //    if ((index == 3) || (index == 4))
            //    {
            //        schemeAct.ActivityType = 1;
            //        if (index == 3)
            //            schemeAct.NameText = "Become a Java coder";
            //        else
            //            schemeAct.NameText = "Advanced techniques in Java";
            //        schemeAct.TypeText = "Datorbaserad";
            //        schemeAct.ActivityId = 1;
            //    }

            //    if ((index == 1) || (index > 7))
            //    {
            //        schemeAct.ActivityType = 3;
            //        if (index == 1)
            //            schemeAct.NameText = "Java övning 1";
            //        else
            //            schemeAct.NameText = "Java övning 2";
            //        schemeAct.TypeText = "Övning";
            //        schemeAct.ActivityId = 1;
            //    }
            //    viewModel.WeekActivities.Add(schemeAct);
            //}

            return View(viewModel);
        }
    }
}