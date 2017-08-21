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
    public enum ActivityRepoResult
    {
        Success,
        BadRequest,
        NotFound,
    }

    public struct SingleActivity
    {
        public Activity activity;
        public ActivityRepoResult repoResult;
    }

    public struct ModuleActivityList
    {
        public List<ActivityListData> activityList;
        public ActivityRepoResult repoResult;
    }

    public struct PeriodActivityList
    {
        public List<PeriodActivityListData> periodActivityList;
        public ActivityRepoResult repoResult;
        public DateTime startOfWeek;
    }

    public static class ActivityRepo
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        // QUERY whether a certain activity (id) is present in DB
        public static bool IsActivityPresent(int? activityId)
        {
            if ((activityId == null) || (activityId == 0))
                return false;

            var activities = db.Activities.Where(a => a.Id == activityId).ToList();
            if (activities.Count != 1)
                return false;

            return true;
        }

        // QUERY whether a certain activity name is valid within a module, i.e. not null and unique against DB
        public static string IsActivityNameValid(int? moduleId, int? activityId, string name)
        {
            if (name == null)
                return "Aktivitetsnamn saknas";

            var activities = db.Activities.Where(a => a.ModuleId == moduleId).Where(a => a.Name == name).ToList();
            if (activities.Count == 0)
                return null;

            if ((activities.Count == 1) && (activities.First().Id == activityId))
                return null;

            return "Denna aktivitet finns redan i modulen";
        }

        // QUERY whether a certain activity span is valid within a module, i.e. not overlapping other activities
        public static string IsActivitySpanValid(int? moduleId, int? activityId, DateTime start, DateTime end)
        {
            // Check if activity selected for weekend
            if ((start.DayOfWeek == DayOfWeek.Saturday) || (start.DayOfWeek == DayOfWeek.Sunday))
                return "Denna aktivitet ligger på en helg";

            var activities = db.Activities.Where(a => a.ModuleId == moduleId).ToList();
            if (activities.Count == 0)
                return null;

            bool overlap = false;
            foreach (var act in activities)
            {
                if (act.Id != activityId)
                {
                    if ((start >= act.StartDate) && (start < act.EndDate))
                        overlap = true;

                    if ((end > act.StartDate) && (end <= act.EndDate))
                        overlap = true;

                    if ((start <= act.StartDate) && (end >= act.EndDate))
                        overlap = true;
                }
            }
            if (!overlap)
                return null;

            return "Denna aktivitet överlappar existerande aktivitet(er)";
        }

        // QUERY whether a certain activity span is valid within a module frame, i.e. not exceeding the module span
        public static string IsActivitySpanInModule(int? courseId, int? moduleId, DateTime start, DateTime end)
        {
            SingleModule singleModule = ModuleRepo.RetrieveModule(courseId, moduleId);
            if ((start < singleModule.module.StartDate) || (end > singleModule.module.EndDate))
                return "Denna aktivitet ligger inte inom modulens start och slut";
            return null;
        }

        // RETREIVE an activity name, non-protected method without RepoResult
        public static string RetrieveActivityName(int? activityId)
        {
            if ((activityId == null) || (activityId == 0))
                return "No name retrieved: Undefined activity ID";

            var activities = db.Activities.Where(a => a.Id == activityId).ToList();
            if (activities.Count != 1)
            {
                return "No name retrieved: Activity ID not found";
            }
            return activities.First().Name;
        }

        // RETREIVE a single activity within a module
        public static SingleActivity RetrieveActivity(int? moduleId, int? activityId)
        {
            var singleActivity = new SingleActivity();

            if ((moduleId == null) || (moduleId == 0) || (activityId == null) || (activityId == 0))
            {
                singleActivity.repoResult = ActivityRepoResult.BadRequest;
                return singleActivity;
            }

            var activities = db.Activities.Where(a => a.ModuleId == moduleId).Where(a => a.Id == activityId).ToList();
            if (activities.Count != 1)
            {
                singleActivity.repoResult = ActivityRepoResult.NotFound;
                return singleActivity;
            }

            singleActivity.activity = activities.First();
            singleActivity.repoResult = ActivityRepoResult.Success;
            return singleActivity;
        }

        // RETREIVE a list of activities (id + name + etc) within a module
        public static ModuleActivityList RetrieveModuleActivityList(int? moduleId)
        {
            var moduleActivityList = new ModuleActivityList();

            if ((moduleId == null) || (moduleId == 0))
            {
                moduleActivityList.repoResult = ActivityRepoResult.BadRequest;
                return moduleActivityList;
            }

            if (!ModuleRepo.IsModulePresent(moduleId))
            {
                moduleActivityList.repoResult = ActivityRepoResult.NotFound;
                return moduleActivityList;
            }

            var activities = db.Activities.Where(a => a.ModuleId == moduleId).ToList();          
            moduleActivityList.activityList = new List<ActivityListData>();
            foreach (var act in activities)
            {
                ActivityListData moduleActivity = new ActivityListData();
                moduleActivity.Id = act.Id;
                moduleActivity.Name = act.Name;
                moduleActivity.Description = act.Description;
                moduleActivity.StartDate = act.StartDate;
                moduleActivity.EndDate = act.EndDate;
                moduleActivityList.activityList.Add(moduleActivity);
            }
            moduleActivityList.repoResult = ActivityRepoResult.Success;
            return moduleActivityList;
        }

        // RETREIVE a list of non-planned periods within a given module time frame
        public static List<AvailableActivityTime> RetrieveActivityFreePeriods(int? moduleId)
        {
            var module = db.Modules.Where(m => m.Id == moduleId).ToList();
            var modStart = module.First().StartDate;
            var modEnd = module.First().EndDate;
            
            var block1start = DateTime.Parse(modStart.ToShortDateString() + " 08:30:00");
            var block1end = DateTime.Parse(modStart.ToShortDateString() + " 12:00:00");
            var block2start = DateTime.Parse(modStart.ToShortDateString() + " 13:00:00");
            var block2end = DateTime.Parse(modStart.ToShortDateString() + " 16:30:00");

            List<AvailableActivityTime> availabilityList = new List<AvailableActivityTime>();

            while (block1start < modEnd)
            {
                bool freeAm = false;
                bool freePm = false;
                ActivityPeriod period;

                if ((block1start >= modStart) && (block1end <= modEnd))
                {
                    freeAm = true;
                    var result = IsActivitySpanValid(moduleId, 0, block1start, block1end);
                    if (result == null)
                    {
                        AvailableActivityTime block = new AvailableActivityTime();
                        block.Start = block1start;
                        block.End = block1end;
                        period = ActivityPeriod.AM;
                        block.Period = (SelectActivityPeriod)period;
                        availabilityList.Add(block);
                    }
                    else
                    {
                        freeAm = false;
                    }
                }
                if ((block2start >= modStart) && (block2end <= modEnd))
                {
                    freePm = true;
                    var result = IsActivitySpanValid(moduleId, 0, block2start, block2end);
                    if (result == null)
                    {
                        AvailableActivityTime block = new AvailableActivityTime();
                        block.Start = block2start;
                        block.End = block2end;
                        period = ActivityPeriod.FM;
                        block.Period = (SelectActivityPeriod)period;
                        availabilityList.Add(block);
                    }
                    else
                    {
                        freePm = false;
                    }
                }
                if (freeAm && freePm)
                {
                    AvailableActivityTime block = new AvailableActivityTime();
                    block.Start = block1start;
                    block.End = block2end;
                    period = ActivityPeriod.FullDay;
                    block.Period = (SelectActivityPeriod)period;
                    availabilityList.Add(block);
                }

                block1start = block1start.AddDays(1);
                block1end = block1end.AddDays(1);
                block2start = block2start.AddDays(1);
                block2end = block2end.AddDays(1);

            }
            return availabilityList;
        }

        // RETREIVE a list of activities (name + start + type) within a course and for a defined time period
        public static PeriodActivityList RetrievePeriodActivities(int? courseId, DateTime start, DateTime  end)
        {
            PeriodActivityList periodActivities = new PeriodActivityList();

            if ((courseId == null) || (courseId == 0))
            {
                periodActivities.repoResult = ActivityRepoResult.BadRequest;
                return periodActivities;
            }
            
            if (!CourseRepo.IsCoursePresent(courseId))
            {
                periodActivities.repoResult = ActivityRepoResult.NotFound;
                return periodActivities;
            }

            periodActivities.startOfWeek = start;
            periodActivities.periodActivityList = new List<PeriodActivityListData>();

            var modules = db.Modules.Where(m => m.CourseId == courseId).Where(m => m.StartDate < end).Where(m => m.EndDate > start).ToList();
            foreach (var mod in modules)
            {
                int moduleId = mod.Id;
                var activities = db.Activities.Where(a => a.ModuleId == moduleId).Where(a => a.StartDate < end).Where(a => a.EndDate > start).ToList();                
                foreach (var act in activities)
                {
                    PeriodActivityListData periodActivity = new PeriodActivityListData();
                    periodActivity.Name = act.Name;
                    periodActivity.ModuleName = mod.Name;
                    periodActivity.StartDate = act.StartDate;
                    periodActivity.ActivityType = (SelectActivityType)act.ActivityType;
                    periodActivity.ActivityPeriod = act.ActivityPeriod;
                    periodActivities.periodActivityList.Add(periodActivity);
                }
            }
            periodActivities.repoResult = ActivityRepoResult.Success;
            return periodActivities;
        }

        // ADD a single activity to a module
        public static int AddActivity(Activity activity)
        {
            NotificationRepo.AddNewActivityNote(activity);

            db.Activities.Add(activity);
            db.SaveChanges();
            return activity.Id;               // Return newly checked out id to caller      
        }

        // UPDATE a single activity within a module
        public static ActivityRepoResult UpdateActivity(Activity activity)
        {
            if ((activity.ModuleId == 0) || (activity.Id == 0))
                return ActivityRepoResult.BadRequest;

            var activities = db.Activities.Where(a => a.ModuleId == activity.ModuleId).Where(a => a.Id == activity.Id).ToList();
            if (activities.Count != 1)
                return ActivityRepoResult.NotFound;

            NotificationRepo.AddChangedActivityNote(activities.First(), activity);

            activities.First().Name = activity.Name;
            activities.First().Description = activity.Description;
            activities.First().StartDate = activity.StartDate;
            activities.First().EndDate = activity.EndDate;
            activities.First().ActivityPeriod = activity.ActivityPeriod;
            activities.First().ActivityType = activity.ActivityType;
            activities.First().Deadline = activity.Deadline;

            db.Entry(activities.First()).State = EntityState.Modified;
            db.SaveChanges();      
            return ActivityRepoResult.Success;     
        }

        // REMOVE a single activity from a module
        public static ActivityRepoResult RemoveActivity(int? moduleId, int? activityId)
        {
            if ((moduleId == null) || (moduleId == 0) || (activityId == null) || (activityId == 0))
                return ActivityRepoResult.BadRequest;

            var activities = db.Activities.Where(a => a.ModuleId == moduleId).Where(a => a.Id == activityId).ToList();
            if (activities.Count != 1)
                return ActivityRepoResult.NotFound;

            NotificationRepo.AddRemovedActivityNote(activities.First());

            db.Activities.Remove(activities.First());
            db.SaveChanges();           
            return ActivityRepoResult.Success;
        }

        // REMOVE all activities from a module
        public static ActivityRepoResult RemoveModuleActivities(int? moduleId)
        {
            if ((moduleId == null) || (moduleId == 0))
                return ActivityRepoResult.BadRequest;

            var activities = db.Activities.Where(a => a.ModuleId == moduleId).ToList();
            if (activities.Count == 0)
                return ActivityRepoResult.NotFound;

            foreach (var act in activities)
            {
                db.Activities.Remove(act);
            }
            db.SaveChanges();
            return ActivityRepoResult.Success;
        }
    }
}