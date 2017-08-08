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

        // RETRIEVE the total time span for all modules within a course
        public static CourseModulesSpan RetrieveCourseSpan(int? courseId)
        {
            var modules = db.Modules.Where(m => m.CourseId == courseId).ToList();
            var courseSpan = new CourseModulesSpan();

            if (modules.Count < 0)
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

        // RETREIVE a list of modules (id + name) within a course
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
                courseModuleList.moduleList.Add(courseModule);
            }
            courseModuleList.repoResult = ModuleRepoResult.Success;
            return courseModuleList;
        }

        // ADD a single module to a course
        public static int AddModule(Module module)
        {
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