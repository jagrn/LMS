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

        // RETREIVE a list of activities (id + name) within a module
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
                moduleActivityList.activityList.Add(moduleActivity);
            }
            moduleActivityList.repoResult = ActivityRepoResult.Success;
            return moduleActivityList;
        }

        // ADD a single activity to a module
        public static int AddActivity(Activity activity)
        {
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

            activities.First().Name = activity.Name;
            activities.First().Description = activity.Description;
            activities.First().StartDate = activity.StartDate;
            activities.First().EndDate = activity.EndDate;
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