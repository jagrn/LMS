using LMS.Repositories;
using LMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS.Controllers
{ 
    public class CommonLOsController : Controller
    {
        // GET: CommonLOs
        public ActionResult HeadBanner(string origin, int? id1, int? id2, int? id3, string linkText0, string linkText1, string linkText2, string linkText3, string infoText)
        {
            HeadBannerViewModel viewModel = new HeadBannerViewModel();
            viewModel.Origin = origin;

            viewModel.Id1 = (id1 == null ? 0 : (int)id1);       // 0 indicates not used
            viewModel.Id2 = (id2 == null ? 0 : (int)id2);       // 0 indicates not used       
            viewModel.Id3 = (id3 == null ? 0 : (int)id3);       // 0 indicates not used

            string defaultLinkText = "";
            switch (origin)
            {
                case "CourseCatalogue":

                    break;
                case "CourseMgmnt":
                    if (id1 != 0)
                        viewModel.LinkText0 = CourseRepo.RetrieveCourseName(id1);
                    else
                        viewModel.LinkText0 = defaultLinkText;
                    break;

                case "ModuleMgmnt":
                    if (id1 != 0)
                        viewModel.LinkText0 = CourseRepo.RetrieveCourseName(id1);
                    else
                        viewModel.LinkText0 = defaultLinkText;
                    if (id2 != 0)
                        viewModel.LinkText1 = ModuleRepo.RetrieveModuleName(id2);
                    else
                        viewModel.LinkText1 = defaultLinkText;
                    break;

                case "ActivityMgmnt":
                    if (id1 != 0)
                        viewModel.LinkText0 = CourseRepo.RetrieveCourseName(id1);
                    else
                        viewModel.LinkText0 = defaultLinkText;
                    if (id2 != 0)
                        viewModel.LinkText1 = ModuleRepo.RetrieveModuleName(id2);
                    else
                        viewModel.LinkText1 = defaultLinkText;
                    if (id3 != 0)
                        viewModel.LinkText2 = ActivityRepo.RetrieveActivityName(id3);
                    else
                        viewModel.LinkText2 = defaultLinkText;
                    break;

                case "DocumentMgmnt":
                    if (id1 != 0)
                        viewModel.LinkText0 = CourseRepo.RetrieveCourseName(id1);
                    else
                        viewModel.LinkText0 = defaultLinkText;
                    if (id2 != 0)
                        viewModel.LinkText1 = ModuleRepo.RetrieveModuleName(id2);
                    else
                        viewModel.LinkText1 = defaultLinkText;
                    if (id3 != 0)
                        viewModel.LinkText2 = ActivityRepo.RetrieveActivityName(id3);
                    else
                        viewModel.LinkText2 = defaultLinkText;
                    break;

                default:
                    break;
            }

            viewModel.InfoText = infoText;

            return PartialView(viewModel);           
        }
    }
}