using LMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LMS.ViewModels
{
    public enum SelectActivityPeriod
    {
        FM,
        EM,
        Heldag,
    }

    public enum SelectActivityType
    {
        Föreläsning,
        Datorbaserad,
        Inlämningsuppgift,
        Övningstillfälle,
    }

    public struct CourseListData
    {
        public int Id;
        public string Name;
    }

    public struct ModuleListData
    {
        public int Id;
        public string Name;
        public string Description;
        public DateTime StartDate;
        public DateTime EndDate;
    }

    public struct ActivityListData
    {
        public int Id;
        public string Name;
        public string Description;
        public DateTime StartDate;
        public DateTime EndDate;
    }

    public struct PeriodActivityListData
    {
        public string Name;
        public string ModuleName;
        public DateTime StartDate;
        public SelectActivityType ActivityType;
        public ActivityPeriod ActivityPeriod;
    }

    public class CourseViewModel
    {
        public int Id { get; set; }
        [DisplayName("Namn")]
        public string Name { get; set; }
        [DisplayName("Beskrivning")]
        public string Description { get; set; }
        [DisplayName("Startdatum")]
        public DateTime StartDate { get; set; }
        [DisplayName("Slutdatum")]
        public DateTime EndDate { get; set; }

        public List<CourseListData> AllCourses { get; set; }
        public int ModuleId { get; set; }
        public List<ModuleListData> CourseModules { get; set; }

        public string PostMessage { get; set; }
        public string PostNavigation { get; set; }
        public string PostOperation { get; set; }
    }

    public class CourseDeleteViewModel
    {
        public int Id { get; set; }
        [DisplayName("Namn")]
        public string Name { get; set; }
        [DisplayName("Beskrivning")]
        public string Description { get; set; }
        [DisplayName("Startdatum")]
        public DateTime StartDate { get; set; }
        [DisplayName("Slutdatum")]
        public DateTime EndDate { get; set; }

        public List<CourseListData> AllCourses { get; set; }

        public string DeleteType { get; set; }
    }

    public class ModuleViewModel
    {
        public int Id { get; set; }
        [DisplayName("Namn")]
        public string Name { get; set; }
        [DisplayName("Beskrivning")]
        public string Description { get; set; }
        [DisplayName("Startdatum")]
        public DateTime StartDate { get; set; }
        [DisplayName("Slutdatum")]
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }
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
        public int Id { get; set; }
        [DisplayName("Namn")]
        public string Name { get; set; }
        [DisplayName("Beskrivning")]
        public string Description { get; set; }
        [DisplayName("Startdatum")]
        public DateTime StartDate { get; set; }
        [DisplayName("Slutdatum")]
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }
        public List<ModuleListData> CourseModules { get; set; }

        public string DeleteType { get; set; }
    }

    public class ActivityViewModel
    {
        public int Id { get; set; }
        [DisplayName("Namn")]
        public string Name { get; set; }
        [DisplayName("Beskrivning")]
        public string Description { get; set; }
        [DisplayName("Startdatum")]
        //
        // följande två rader ger en datepicker till startDate
        //
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        [DisplayName("Period")]
        public SelectActivityPeriod Period { get; set; }
        public DateTime Deadline { get; set; }
        [DisplayName("Aktivitetstyp")]
        public SelectActivityType SelectActivityType { get; set; }

        public int CourseId { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public List<ActivityListData> ModuleActivities { get; set; }

        public string PostMessage { get; set; }
        public string PostNavigation { get; set; }
        public string PostOperation { get; set; }
    }

    public class ActivityDeleteViewModel
    {
        public int Id { get; set; }
        [DisplayName("Namn")]
        public string Name { get; set; }
        [DisplayName("Beskrivning")]
        public string Description { get; set; }
        [DisplayName("Startdatum")]
        public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        public SelectActivityPeriod Period { get; set; }
        public DateTime Deadline { get; set; }
        [DisplayName("Aktivitetstyp")]
        public SelectActivityType SelectActivityType { get; set; }

        public int CourseId { get; set; }
        public int ModuleId { get; set; }
        public List<ActivityListData> ModuleActivities { get; set; }

        public string DeleteType { get; set; }
    }


    public class StudentViewModel
    {
        public int CourseId { get; set; }
        public int ModuleId { get; set; }
        public int ActivityId { get; set; }
        public string CourseName { get; set; }      
        public Module Module { get; set; }
        public Activity Activity { get; set; }
        public List<ModuleListData> CourseModules { get; set; }     
        public List<ActivityListData> ModuleActivities { get; set; }      
    }

        public struct SchemeActivity
    {
        public int ActivityType;    // According to SelectActivityType and -1 => "Ledig"
        public string NameText;
        public string TypeText;
        public string ModuleNameText;
        public int ActivityId;
    }

    public class SchemeViewModel
    {
        public int courseId;
        public List<SchemeActivity> WeekActivities { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public DateTime Monday { get; set; }
        public string Period { get; set; }

        public List<Notification> Notifications { get; set; }
    }


}