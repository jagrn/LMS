using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.ViewModels
{
    public class HeadBannerViewModel
    {
        public string Type { get; set; }

        public int Id1 { get; set; }
        public int Id2 { get; set; }
        public int Id3 { get; set; }

        public string LinkText0 { get; set; }
        public string LinkText1 { get; set; }
        public string LinkText2 { get; set; }
        public string LinkText3 { get; set; }

        public string InfoText { get; set; }
    }
}