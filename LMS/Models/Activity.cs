using System;

namespace LMS.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Deadline { get; set; }
        public ActivityType ActitvityType { get; set; }
        public int ModuleId { get; set; }

    }

    public enum ActivityType
    {
        Lecture,
        E_Learning,
        Task,
        Excercise
    }
}

