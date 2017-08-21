using System;
using LMS.Models;

namespace LMS.Repositories
{    
    public static class dbRepo
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// a couple of methods to fetch coursename, modulename, activityname etc from activityId, moduleId etc
        /// </summary>
        /// 
        public static string ActivityId2courseName(int? activityId)
        {
            return db.Courses.Find(db.Modules.Find(db.Activities.Find(activityId).ModuleId).CourseId).Name;
        }

        public static string ActivityId2moduleName(int? activityId)
        {
            return db.Modules.Find(db.Activities.Find(activityId).ModuleId).Name;
        }

        public static string ActivityId2activityName(int? activityId)
        {
            return db.Activities.Find(activityId).Name;
        }

        public static string ModuleId2courseName(int? moduleId)
        {
            return db.Courses.Find(db.Modules.Find(moduleId).CourseId).Name;
        }
        public static string ModuleId2moduleName(int? moduleId)
        {
            return db.Modules.Find(moduleId).Name;
        }
        public static string CourseId2courseName(int? courseId)
        {
            return db.Courses.Find(courseId).Name;
        }

        internal static int? ActivityId2courseId(int? activityId)
        {
            return db.Courses.Find(db.Modules.Find(db.Activities.Find(activityId).ModuleId).CourseId).Id;
        }

        internal static int? ActivityId2moduleId(int? activityId)
        {
            return db.Modules.Find(db.Activities.Find(activityId).ModuleId).Id;
        }

        internal static int? ModuleId2courseId(int? moduleId)
        {
            return db.Courses.Find(db.Modules.Find(moduleId).CourseId).Id;
        }
    }
}