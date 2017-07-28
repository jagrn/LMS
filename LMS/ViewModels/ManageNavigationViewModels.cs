using LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.ViewModels
{
    public struct ModuleListData
	{
        public int Id;
        public string Name;
	}

    public struct ActivityListData
    {
        public int Id;
        public string Name;
    }

    public class ModuleViewModel
    {
        // Model parameters, possibly filtered to expose a subset to the view.
        public int Id { get; set; }        
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CourseId { get; set; }

        // Additional display/control parameters.
        public string CourseName { get; set; }
        public List<ModuleListData> CourseModules { get; set; }
        public int ActivityId { get; set; }
        public List<ActivityListData> ModuleActivities { get; set; }
        public string PostMessage { get; set; }
        public string PostNavigation { get; set; }
        public string PostOperation { get; set; }     
    }

    public class ModuleDeleteViewModel
    {
        // Model parameters, filtered to expose a subset to the view.
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CourseId { get; set; }

        // Additional display/control parameters.
        public string DeleteType { get; set; }
        public List<ModuleListData> CourseModules { get; set; }
    }

    public class ActivityViewModel
    {
        // Model parameters, possibly filtered to expose a subset to the view.
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Deadline { get; set; }
        public ActivityType ActitvityType { get; set; }
        public int ModuleId { get; set; }

        // Additional display/control parameters.
        public string ModuleName { get; set; }
        public List<ActivityListData> ModuleActivities { get; set; }
        public int CourseId { get; set; }
        public string PostMessage { get; set; }
        public string PostNavigation { get; set; }
        public string PostOperation { get; set; }
    }

    public class ActivityDeleteViewModel
    {
        // Model parameters, filtered to expose a subset to the view.
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Deadline { get; set; }
        public ActivityType ActitvityType { get; set; }
        public int ModuleId { get; set; }

        // Additional display/control parameters.
        public string DeleteType { get; set; }
        public List<ActivityListData> ModuleActivities { get; set; }
        public int CourseId { get; set; }
    }
}