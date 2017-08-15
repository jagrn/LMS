using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int CourseId { get; set; }

        public DateTime ChangeTime { get; set; }
        public DateTime EndOfRelevance { get; set; }
        public string ChangeText { get; set; }
    }
}