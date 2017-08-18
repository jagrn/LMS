using System;
using System.Collections.Generic;

namespace LMS.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ActivityPeriod ActivityPeriod  { get; set; }
        public DateTime Deadline { get; set; }
        public ActivityType ActivityType { get; set; }
        public int ModuleId { get; set; }
        public virtual ICollection<Document> Documents { get; set; }

    }

    public enum ActivityPeriod
    {
        AM,
        FM,
        FullDay,
    }

    public enum ActivityType
    {
        Lecture,
        E_Learning,
        Task,
        Excercise
    }
}

