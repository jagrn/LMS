using LMS.Models;
using LMS.ViewModels;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LMS.Repositories
{
    public enum ModuleRepoResult
    {
        Success,
        BadRequest,
        NotFound,
    }

    public struct SingleModule
    {
        public Module module;
        public ModuleRepoResult repoResult;
    }

    public struct CourseModuleList
    {
        public List<ModuleListData> moduleList;
        public ModuleRepoResult repoResult;
    }

    public struct CourseModulesSpan
    {
        public DateTime start;
        public DateTime end;
    }

    public class ModuleRepo
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        // QUERY whether a certain module (id) is present in DB
        public static bool IsModulePresent(int? moduleId)
        {
            if ((moduleId == null) || (moduleId == 0))
                return false;

            var modules = db.Modules.Where(m => m.Id == moduleId).ToList();
            if (modules.Count != 1)
                return false;

            return true;
        }

        // QUERY whether a certain module name is valid within a course, i.e. not null and unique against DB
        public static string IsModuleNameValid(int? courseId, int? moduleId, string name)
        {
            if (name == null)
                return "Modulnamn saknas";

            var modules = db.Modules.Where(m => m.CourseId == courseId).Where(m => m.Name == name).ToList();
            if (modules.Count == 0)
                return null;

            if ((modules.Count == 1) && (modules.First().Id == moduleId))
                return null;

            return "Denna modul finns redan i kursen";
        }

        // QUERY whether a certain module span is valid within a course, i.e. not overlapping other modules
        public static string IsModuleSpanValid(int? courseId, int? moduleId, DateTime start, DateTime end)
        {
            var modules = db.Modules.Where(m => m.CourseId == courseId).ToList();
            if (modules.Count == 0)
                return null;

            bool overlap = false;
            foreach (var mod in modules)
            {
                if (mod.Id != moduleId)
                {
                    if ((start >= mod.StartDate) && (start < mod.EndDate))
                        overlap = true;

                    if ((end > mod.StartDate) && (end <= mod.EndDate))
                        overlap = true;

                    if ((start <= mod.StartDate) && (end >= mod.EndDate))
                        overlap = true;
                }
            }
            if (!overlap)
                return null;

            return "Denna modul överlappar existerande modul(er)";
        }

        // RETREIVE a module name, non-protected method without RepoResult
        public static string RetrieveModuleName(int? moduleId)
        {
            if ((moduleId == null) || (moduleId == 0))
                return "No name retrieved: Undefined module ID";

            var modules = db.Modules.Where(m => m.Id == moduleId).ToList();
            if (modules.Count != 1)
            {
                return "No name retrieved: Module ID not found";
            }
            return modules.First().Name;
        }

        // RETREIVE a modules course Id, non-protected method without RepoResult
        public static int RetrieveModuleCourseId(int? moduleId)
        {
            if ((moduleId == null) || (moduleId == 0))
                return 0;

            var modules = db.Modules.Where(m => m.Id == moduleId).ToList();
            if (modules.Count != 1)
            {
                return -1;
            }
            return modules.First().CourseId;
        }

        // RETRIEVE the total time span for all modules within a course
        public static CourseModulesSpan RetrieveCourseSpan(int? courseId)
        {
            var modules = db.Modules.Where(m => m.CourseId == courseId).ToList();
            var courseSpan = new CourseModulesSpan();

            if (modules.Count > 0)
            {
                courseSpan.start = modules.First().StartDate;
                courseSpan.end = modules.First().EndDate;
                foreach (var mod in modules)
                {
                    if (mod.StartDate < courseSpan.start)
                    {
                        courseSpan.start = mod.StartDate;
                    }
                    if (mod.EndDate > courseSpan.end)
                    {
                        courseSpan.end = mod.EndDate;
                    }
                }
            }
            else
            {
                courseSpan.start = DateTime.Parse("2017-01-01 00:00:00");
                courseSpan.end = DateTime.Parse("2017-01-01 00:00:00");
            }

            return courseSpan;
        }

        // RETREIVE a single module within a course
        public static SingleModule RetrieveModule(int? courseId, int? moduleId)
        {
            var singleModule = new SingleModule();

            if ((courseId == null) || (courseId == 0) || (moduleId == null) || (moduleId == 0))
            {
                singleModule.repoResult = ModuleRepoResult.BadRequest;
                return singleModule;
            }

            var modules = db.Modules.Where(m => m.CourseId == courseId).Where(m => m.Id == moduleId).ToList();
            if (modules.Count != 1)
            {
                singleModule.repoResult = ModuleRepoResult.NotFound;
                return singleModule;
            }

            singleModule.module = modules.First();
            singleModule.repoResult = ModuleRepoResult.Success;
            return singleModule;
        }

        // RETREIVE a list of modules (id + name + etc) within a course
        public static CourseModuleList RetrieveCourseModuleList(int? courseId)
        {
            var courseModuleList = new CourseModuleList();

            if ((courseId == null) || (courseId == 0))
            {
                courseModuleList.repoResult = ModuleRepoResult.BadRequest;
                return courseModuleList;
            }

            if (!CourseRepo.IsCoursePresent(courseId))
            {
                courseModuleList.repoResult = ModuleRepoResult.NotFound;
                return courseModuleList;
            }

            var modules = db.Modules.Where(m => m.CourseId == courseId).ToList();
            courseModuleList.moduleList = new List<ModuleListData>();
            foreach (var mod in modules)
            {
                ModuleListData courseModule = new ModuleListData();
                courseModule.Id = mod.Id;
                courseModule.Name = mod.Name;
                courseModule.Description = mod.Description;
                courseModule.StartDate = mod.StartDate;
                courseModule.EndDate = mod.EndDate;
                courseModuleList.moduleList.Add(courseModule);
            }
            courseModuleList.repoResult = ModuleRepoResult.Success;
            return courseModuleList;
        }

        // RETREIVE number of activities in module
        public static int RetrieveNoOfActivities(int moduleId)
        {
            var module = db.Modules.Where(m => m.Id == moduleId).ToList();
            if (module.First().Activities == null)
                return 0;
            return module.First().Activities.Count();
        }

        // Helper to RetrieveModuleFreePeriods()
        private static DateTime AdjustStart(DateTime end, string validStart, string validEnd)
        {
            var start = end;
            var refEnd = DateTime.Parse(end.ToShortDateString() + validEnd);
            bool movedDay = false;

            if (start >= refEnd)                         // Skip outside working day
            {
                start = start.AddDays(1);
                movedDay = true;
            }

            var dayOffset = ((int)(start.DayOfWeek + 6)) % 7;
            while (dayOffset > 4)                       // Skip weekend
            {
                start = start.AddDays(1);
                movedDay = true;
                dayOffset--;
            }
            if (movedDay)
            {
                start = DateTime.Parse(start.ToShortDateString() + validStart);
            }
            return start;
        }

        // Helper to RetrieveModuleFreePeriods()
        private static DateTime AdjustEnd(DateTime start, string validStart, string validEnd)
        {
            var end = start;
            var refStart = DateTime.Parse(start.ToShortDateString() + validStart);
            bool movedDay = false;

            if (end <= refStart)                       // Skip outside working day
            {
                end = end.AddDays(-1);
                movedDay = true;
            }

            var dayOffset = ((int)(end.DayOfWeek + 6)) % 7;
            while (dayOffset > 4)                       // Skip weekend
            {
                end = end.AddDays(-1);
                movedDay = true;
                dayOffset--;
            }
            if (movedDay)
            {
                end = DateTime.Parse(end.ToShortDateString() + validEnd);
            }
            return end;
        }

        // RETREIVE a list of non-planned periods within a given course time frame and including outside the course
        public static List<AvailableModuleTime> RetrieveModuleFreePeriods(int? courseId)
        {           
            var course = db.Courses.AsNoTracking().Where(c => c.Id == courseId).ToList();
            string validStart = " 08:30:00";
            string validEnd = " 16:30:00";

            List<AvailableModuleTime> availabilityList = new List<AvailableModuleTime>();
            AvailableModuleTime period = new AvailableModuleTime();

            if (course.First().Modules.Count == 0)         // No modules defined for course
            {
                period.FixedStart = false;
                period.FixedEnd = false;
                availabilityList.Add(period);
                return availabilityList;
            }
   
            period.FixedStart = false;
            period.FixedEnd = true;
            period.End = AdjustEnd(course.First().StartDate, validStart, validEnd);
            availabilityList.Add(period);

            var modules = db.Modules.Where(m => m.CourseId == courseId).ToList();
            var noOfModules = modules.Count();
            
            for (var i = 0; i < noOfModules-1; i++)
            {
                period.FixedStart = true;
                period.FixedEnd = true;
                period.Start = AdjustStart(modules.ElementAt(i).EndDate, validStart, validEnd);
                period.End = AdjustEnd(modules.ElementAt(i+1).StartDate, validStart, validEnd);
                availabilityList.Add(period);
            }

            period.FixedStart = true;
            period.FixedEnd = false;
            period.Start = AdjustStart(course.First().EndDate, validStart, validEnd);
            availabilityList.Add(period);

            return availabilityList;
        }


        // ADD a single module to a course
        public static int AddModule(Module module)
        {
            NotificationRepo.AddNewModuleNote(module);
            db.Modules.Add(module);
            db.SaveChanges();
            return module.Id;               // Return newly checked out id to caller      
        }

        // UPDATE a single module within a course
        public static ModuleRepoResult UpdateModule(Module module)
        {
            if ((module.CourseId == 0) || (module.Id == 0))
                return ModuleRepoResult.BadRequest;

            var modules = db.Modules.Where(m => m.CourseId == module.CourseId).Where(m => m.Id == module.Id).ToList();
            if (modules.Count != 1)
                return ModuleRepoResult.NotFound;

            NotificationRepo.AddChangedModuleNote(modules.First(), module);
            modules.First().Name = module.Name;
            modules.First().Description = module.Description;
            modules.First().StartDate = module.StartDate;
            modules.First().EndDate = module.EndDate;

            db.Entry(modules.First()).State = EntityState.Modified;
            db.SaveChanges();
            return ModuleRepoResult.Success;
        }

        // REMOVE a single module from a course
        public static ModuleRepoResult RemoveModule(int? courseId, int? moduleId)
        {
            if ((courseId == null) || (courseId == 0) || (moduleId == null) || (moduleId == 0))
                return ModuleRepoResult.BadRequest;

            var modules = db.Modules.Where(m => m.CourseId == courseId).Where(m => m.Id == moduleId).ToList();
            if (modules.Count != 1)
                return ModuleRepoResult.NotFound;

            NotificationRepo.AddRemovedModuleNote(modules.First());
            db.Modules.Remove(modules.First());
            db.SaveChanges();
            return ModuleRepoResult.Success;
        }

        // REMOVE all modules from a course
        public static ModuleRepoResult RemoveCourseModules(int? courseId)
        {
            if ((courseId == null) || (courseId == 0))
                return ModuleRepoResult.BadRequest;

            var modules = db.Modules.Where(m => m.CourseId == courseId).ToList();
            if (modules.Count == 0)
                return ModuleRepoResult.NotFound;

            foreach (var mod in modules)
            {
                db.Modules.Remove(mod);
            }
            db.SaveChanges();
            return ModuleRepoResult.Success;
        }
    }
}