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
        public ActionResult HeadBanner(string type, int? id1, int? id2, int? id3, string linkText0, string linkText1, string linkText2, string linkText3, string infoText)
        {
            HeadBannerViewModel viewModel = new HeadBannerViewModel();
            viewModel.Type = type;
            viewModel.Id1 = (id1 == null ? 0 : (int)id1);       // 0 indicates not used
            viewModel.Id2 = (id2 == null ? 0 : (int)id2);       // 0 indicates not used       
            viewModel.Id3 = (id3 == null ? 0 : (int)id3);       // 0 indicates not used

            viewModel.LinkText0 = linkText0;
            viewModel.LinkText1 = linkText1;
            viewModel.LinkText2 = linkText2;
            viewModel.LinkText3 = linkText3;
            viewModel.InfoText = infoText;

            return PartialView(viewModel);           
        }
    }
}