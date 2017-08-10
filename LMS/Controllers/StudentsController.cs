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
        public ActionResult Scheme()
        {
            SchemeViewModel viewModel = new SchemeViewModel();
            viewModel.Year = 2017;
            viewModel.Week = 10;
            viewModel.Period = "2017-04-10 -- 2017-04-15";
            viewModel.WeekActivities = new List<SchemeActivity>();

            int index = 0;
            SchemeActivity schemeAct = new SchemeActivity();
            foreach (var viewMod in viewModel.WeekActivities)
            {
                if (index == 0)
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
                index++;
            }

            return View(viewModel);
        }
    }
}