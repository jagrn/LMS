﻿using System;
using System.Collections.Generic;

namespace LMS.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual ICollection<Module> Modules { get; set; }
        public virtual ICollection<ApplicationUser> Students { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
    }
}