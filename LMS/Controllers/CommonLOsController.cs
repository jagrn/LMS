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
    public class CommonLOsController : Controller
    {
        // GET: CommonLOs/HeadBanner
        public ActionResult HeadBanner(string origin, int? id1, int? id2, int? id3, string linkText0, string linkText1, string linkText2, string linkText3, string infoText)
        {
            HeadBannerViewModel viewModel = new HeadBannerViewModel();
            viewModel.Origin = origin;

            viewModel.Id1 = (id1 == null ? 0 : (int)id1);       // 0 indicates not used
            viewModel.Id2 = (id2 == null ? 0 : (int)id2);       // 0 indicates not used       
            viewModel.Id3 = (id3 == null ? 0 : (int)id3);       // 0 indicates not used

            string defaultLinkText = "";
            viewModel.LinkText0 = defaultLinkText;
            viewModel.LinkText1 = defaultLinkText;
            viewModel.LinkText2 = defaultLinkText;

            switch (origin)
            {
                case "CourseCatalogue":

                    break;
                case "CourseMgmnt":
                    if (id1 != 0)
                        viewModel.LinkText0 = CourseRepo.RetrieveCourseName(id1);
                    break;

                case "ModuleMgmnt":
                    if (id1 != 0)
                        viewModel.LinkText0 = CourseRepo.RetrieveCourseName(id1);                
                    if (id2 != 0)
                        viewModel.LinkText1 = ModuleRepo.RetrieveModuleName(id2);                  
                    break;

                case "ActivityMgmnt":
                    if (id1 != 0)
                        viewModel.LinkText0 = CourseRepo.RetrieveCourseName(id1);                  
                    if (id2 != 0)
                        viewModel.LinkText1 = ModuleRepo.RetrieveModuleName(id2);                 
                    if (id3 != 0)
                        viewModel.LinkText2 = ActivityRepo.RetrieveActivityName(id3);                  
                    break;

                case "DocumentMgmnt":
                    if (id1 != 0)
                        viewModel.LinkText0 = CourseRepo.RetrieveCourseName(id1);                   
                    if (id2 != 0)
                        viewModel.LinkText1 = ModuleRepo.RetrieveModuleName(id2);                    
                    if (id3 != 0)
                        viewModel.LinkText2 = ActivityRepo.RetrieveActivityName(id3);                   
                    break;

                default:
                    break;
            }

            viewModel.InfoText = infoText;
            return PartialView(viewModel);           
        }

        public ActionResult FootBanner(string origin, int? id1, int? id2, int? id3, string linkText0, string linkText1, string linkText2, string linkText3, string infoText)
        {
            FootBannerViewModel viewModel = new FootBannerViewModel();
            viewModel.Origin = origin;

            viewModel.Id1 = (id1 == null ? 0 : (int)id1);       // 0 indicates not used
            viewModel.Id2 = (id2 == null ? 0 : (int)id2);       // 0 indicates not used       
            viewModel.Id3 = (id3 == null ? 0 : (int)id3);       // 0 indicates not used

            string defaultLinkText = "";
            viewModel.LinkText0 = defaultLinkText;
            viewModel.LinkText1 = defaultLinkText;
            viewModel.LinkText2 = defaultLinkText;

            switch (origin)
            {
                case "CourseCatalogue":

                    break;
                case "CourseMgmnt":
                    if (id1 != 0)
                        viewModel.LinkText0 = CourseRepo.RetrieveCourseName(id1);
                    break;

                case "ModuleMgmnt":
                    if (id1 != 0)
                        viewModel.LinkText0 = CourseRepo.RetrieveCourseName(id1);
                    if (id2 != 0)
                        viewModel.LinkText1 = ModuleRepo.RetrieveModuleName(id2);
                    break;

                case "ActivityMgmnt":
                    if (id1 != 0)
                        viewModel.LinkText0 = CourseRepo.RetrieveCourseName(id1);
                    if (id2 != 0)
                        viewModel.LinkText1 = ModuleRepo.RetrieveModuleName(id2);
                    if (id3 != 0)
                        viewModel.LinkText2 = ActivityRepo.RetrieveActivityName(id3);
                    break;

                case "DocumentMgmnt":
                    if (id1 != 0)
                        viewModel.LinkText0 = CourseRepo.RetrieveCourseName(id1);
                    if (id2 != 0)
                        viewModel.LinkText1 = ModuleRepo.RetrieveModuleName(id2);
                    if (id3 != 0)
                        viewModel.LinkText2 = ActivityRepo.RetrieveActivityName(id3);
                    break;

                default:
                    break;
            }

            viewModel.InfoText = infoText;
            return PartialView(viewModel);
        }

        // GET: CommonLOs/ShowList
        //public ActionResult UpdateList(string listType, int objectId)
        //    {
        //        switch (listType)
        //        {
        //            case "ModuleList":
        //                UpdateModulesViewModel moduleViewModel = new UpdateModulesViewModel();
        //                var courseModuleList = ModuleRepo.RetrieveCourseModuleList(objectId);
        //                moduleViewModel.CourseModules = courseModuleList.moduleList;
        //                return PartialView(moduleViewModel);
        //                break;

        //            case "ActivityList":
        //                UpdateActivitiesViewModel activityViewModel = new UpdateActivitiesViewModel();
        //                var moduleActivitiesList = ActivityRepo.RetrieveModuleActivityList(objectId);
        //                activityViewModel.ModuleActivities = moduleActivitiesList.activityList;
        //                return PartialView(activityViewModel);
        //                break;



        //        }

        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
    }
}