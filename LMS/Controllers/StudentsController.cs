using LMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS.Controllers
{
    public class StudentsController : Controller
    {
        // GET: Students/Scheme
        public ActionResult Scheme(int? courseId, int? year, int? week, int? moveWeek)
        {
            SchemeViewModel viewModel = new SchemeViewModel();
            if (courseId == null)
            {
                viewModel.Year = 2017;
                viewModel.Week = 10;
            }
            else
            {
                viewModel.Year = (int)year + 1;
                viewModel.Week = (int)week + (int)moveWeek;
            }
            viewModel.Period = "2017-04-10 -- 2017-04-15";
            viewModel.WeekActivities = new List<SchemeActivity>();

            for (int index = 0; index < 10; index++)
            {
                SchemeActivity schemeAct = new SchemeActivity();
                schemeAct.ActivityType = index-1;
                schemeAct.NameText = "NameText " + index.ToString();
                schemeAct.TypeText = "TypeText " + index.ToString();
                schemeAct.ActivityId = index;
                viewModel.WeekActivities.Add(schemeAct);
            }


            

            return View(viewModel);
        }
    }
}