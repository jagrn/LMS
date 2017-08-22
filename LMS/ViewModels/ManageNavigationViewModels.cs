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

    public struct DocumentListData
    {
        public int Id;
        public string Name;
        public string Description;
        public DocumentType DocumentType;
        public DateTime UploadDate;
    }


    public struct PeriodActivityListData
    {
        public string Name;
        public string ModuleName;
        public DateTime StartDate;
        public SelectActivityType ActivityType;
        public ActivityPeriod ActivityPeriod;
    }

    public struct AvailableActivityTime
    {
        public DateTime Start;
        public DateTime End;
        public SelectActivityPeriod Period;
    }

    public struct AvailableModuleTime
    {
        public DateTime Start;
        public DateTime End;
        public bool FixedStart;
        public bool FixedEnd;
    }

    public class CourseViewModel
    {
        public int Id { get; set; }
        [DisplayName("Kursnamn")]
        public string Name { get; set; }
        [DisplayName("Beskrivning")]
        public string Description { get; set; }
        [DisplayName("Startdatum")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", 
            ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DisplayName("Slutdatum")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", 
            ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public List<CourseListData> AllCourses { get; set; }
        public int ModuleId { get; set; }
        public List<ModuleListData> CourseModules { get; set; }
        public int NoOfModules { get; set; }
        public bool ShowModules { get; set; }

        public List<DocumentListData> CourseDocuments { get; set; }
        public int NoOfDocuments { get; set; }
        public bool ShowDocuments { get; set; }

        public string PostMessage { get; set; }
        public string PostNavigation { get; set; }
        public string PostOperation { get; set; }

        //public List<Module> TestCourseModules1 { get; set; }             // TEMP TEST
        //public List<Module> TestCourseModules2 { get; set; }             // TEMP TEST
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
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DisplayName("Slutdatum")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public List<ModuleListData> CourseModules { get; set; }
        public int ActivityId { get; set; }
        public List<ActivityListData> ModuleActivities { get; set; }
        public int NoOfActivities { get; set; }
        public bool ShowActivities { get; set; }

        public List<DocumentListData> ModuleDocuments { get; set; }
        public int NoOfDocuments { get; set; }
        public bool ShowDocuments { get; set; }

        public List<AvailableModuleTime> AvailableTime { get; set; }

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

        public List<DocumentListData> ActivityDocuments { get; set; }
        public int NoOfDocuments { get; set; }
        public bool ShowDocuments { get; set; }

        public List<AvailableActivityTime> AvailableTime { get; set; }

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
        public string StudentId { get; set; }
        public string CourseName { get; set; }
        public string StudentName { get; set; }
        public Module Module { get; set; }
        public Activity Activity { get; set; }
        public List<ModuleListData> CourseModules { get; set; }     
        public List<ActivityListData> ModuleActivities { get; set; }
        public int NoOfNotifications { get; set; }
        public List<Notification> Notifications { get; set; }

        public int SchemeYear { get; set; }
        public int SchemeWeek { get; set; }
        public int? SchemeMoveWeek { get; set; }
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

        public int MyPageModuleId { get; set; }
        public int MyPageActivityId { get; set; }
        public string MyPageStudentId { get; set; }

        public List<SchemeActivity> WeekActivities { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public DateTime Monday { get; set; }
        public string Period { get; set; }

        public List<Notification> Notifications { get; set; }
    }

    public class DocumentViewModel
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Namn")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Beskrivning")]
        public string Description { get; set; }
        [DisplayName("Dokumenttyp")]
        public DocumentType DocumentType { get; set; }
        public string Format { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Skapat datum")]
        public DateTime UploadDate { get; set; }
        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? ActivityId { get; set; }
        public string UserId { get; set; }
        // virtual fields fetched from other tables
        [DisplayName("Kurs")]
        public string CourseName { get; set; }
        [DisplayName("Modul")]
        public string ModuleName { get; set; }
        [DisplayName("Aktivitet")]
        public string ActivityName { get; set; }
        [DisplayName("Kurs/modul/aktivitet")]
        public string LongCourseName { get; set; }
        [DisplayName("Ägare")]
        public string UserName { get; set; }
        // the following four used only to satisfy requirement from course-manage module-manage,  activity-manage which requires these as parameters
        public int? _CourseId { get; set; }
        public int? _ModuleId { get; set; }
        public int? _ActivityId { get; set; }
        public string _UserId { get; set; }
        // virtual fields for passing info back n forth
        public string PostMessage { get; set; }
        public string PostNavigation { get; set; }
        public string PostOperation { get; set; }
        // list of the other documents having the same owner (course or module etc...)
        public List<DocumentListData> SiblingDocuments { get; set; }
    }

    public class DocumentDeleteViewModel
    {
        public int Id { get; set; }
        [DisplayName("Namn")]
        public string Name { get; set; }
        [DisplayName("Dokumenttyp")]
        public DocumentType DocumentType { get; set; }
        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? ActivityId { get; set; }
    }
}