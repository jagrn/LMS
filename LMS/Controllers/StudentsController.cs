using LMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
            periodData.Start = nextMonday;
            periodData.End = nextMonday.AddDays(4);
            periodData.Year = nextMonday.Year;
            periodData.Week = GetWeek(nextMonday);
            return periodData;
        }


        public ActionResult About(int? courseId, int? year, int? week, int? moveWeek)
        {
            ViewBag.Message = "Your application description page.";

            return View(GetSchemeViewModel(courseId, year, week, moveWeek));
        }

        private SchemeViewModel GetSchemeViewModel(int? courseId, int? year, int? week, int? moveWeek)
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
            viewModel.WeekActivities = new List<SchemeActivity>();

            for (int index = 0; index < 10; index++)
            {
                SchemeActivity schemeAct = new SchemeActivity();
                if ((index == 0) || (index == 5) || (index == 6) || (index == 7))
                {
                    schemeAct.ActivityType = -1;
                    schemeAct.NameText = "";
                    schemeAct.TypeText = "";
                    schemeAct.ActivityId = 1;
                }

                if (index == 2)
                {
                    schemeAct.ActivityType = 0;
                    schemeAct.NameText = "Java introduction";
                    schemeAct.TypeText = "Föreläsning";
                    schemeAct.ActivityId = 1;
                }

                if ((index == 3) || (index == 4))
                {
                    schemeAct.ActivityType = 1;
                    if (index == 3)
                        schemeAct.NameText = "Become a Java coder";
                    else
                        schemeAct.NameText = "Advanced techniques in Java";
                    schemeAct.TypeText = "Datorbaserad";
                    schemeAct.ActivityId = 1;
                }

                if ((index == 1) || (index > 7))
                {
                    schemeAct.ActivityType = 3;
                    if (index == 1)
                        schemeAct.NameText = "Java övning 1";
                    else
                        schemeAct.NameText = "Java övning 2";
                    schemeAct.TypeText = "Övning";
                    schemeAct.ActivityId = 1;
                }
                viewModel.WeekActivities.Add(schemeAct);
            }
            
            return viewModel;
        }


        // GET: Students/Scheme
        public ActionResult SchemePartial(int? courseId, int? year, int? week, int? moveWeek)
        {
            return View("Scheme", GetSchemeViewModel(courseId, year, week, moveWeek));
            //return PartialView("_Scheme", GetSchemeViewModel(courseId, year, week, moveWeek));
        }


        public ActionResult Scheme(int? courseId, int? year, int? week, int? moveWeek)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Scheme", GetSchemeViewModel(courseId, year, week, moveWeek));
            }
            return View("Scheme",GetSchemeViewModel(courseId, year, week, moveWeek));
        }

        //[HttpPost]
        //public ActionResult Scheme()
        //{
        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView("_Scheme");
        //    }
        //    return View("Scheme");
        //}

    }
}