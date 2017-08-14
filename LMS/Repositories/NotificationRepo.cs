using LMS.Models;
using LMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Repositories
{
    public class NotificationRepo
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        // RETRIEVE a list of notifications for a certain course
        public static List<Notification> RetrieveCourseNotes(int courseId)
        {
            var notifications = db.Notifications.Where(n => n.CourseId == courseId).ToList();
            return notifications;
        }

        // RETRIEVE a list of relevant notifications for a certain course
        public static List<Notification> RetrieveRelevantCourseNotes(int courseId)
        {
            var notifications = db.Notifications.Where(n => n.CourseId == courseId).Where(n => n.EndOfRelevance >= DateTime.Now).ToList();
            return notifications;
        }

        // AS A GENERAL RULE, DO NOT LOG CHANGES UNLESS AT LEAST ONE STUDENT IS PRESENT IN COURSE.

        // ADD a notification about a new module
        public static void AddNewModuleNote(Module module)
        {
            if (DateTime.Now > module.EndDate)
                return;                             // No logging of changes in the passed

            Notification notification = new Notification();
            notification.CourseId = module.CourseId;
            notification.ChangeTime = DateTime.Now;
            notification.EndOfRelevance = module.EndDate;

            string changeText = "En ny modul är tillagd: " + module.Name + "\n";
            changeText += "Start: " + module.StartDate.ToShortDateString() + "\n";
            changeText += "Slut: " + module.EndDate.ToShortDateString() + "\n";
            notification.ChangeText = changeText;

            db.Notifications.Add(notification);
            db.SaveChanges();

            // CALL A STUDENT REPO METHOD TO UPDATE ALL STUDENTS OF THE COURSE WITH THIS ITEM TO myNotes
        }

        // ADD a notification about a removed module
        public static void AddRemovedModuleNote(Module module)
        {
            if (DateTime.Now > module.EndDate)
                return;                             // No logging of changes in the passed

            Notification notification = new Notification();
            notification.CourseId = module.CourseId;
            notification.ChangeTime = DateTime.Now;
            notification.EndOfRelevance = module.EndDate;

            string changeText = "En modul är borttagen: " + module.Name + "\n";
            changeText += "Start: " + module.StartDate.ToShortDateString() + "\n";
            changeText += "Slut: " + module.EndDate.ToShortDateString() + "\n";
            changeText += "Följande aktiviteter har utgått: ";

            var activities = ActivityRepo.RetrieveModuleActivityList(module.Id).activityList;
            foreach (var act in activities)
            {
                changeText += act.Name + " \n";
            }
            notification.ChangeText = changeText;

            db.Notifications.Add(notification);
            db.SaveChanges();

            // CALL A STUDENT REPO METHOD TO UPDATE ALL STUDENTS OF THE COURSE WITH THIS ITEM TO myNotes
        }

        // ADD a notification about a changed module
        public static void AddChangedModuleNote(Module oldModule, Module newModule)
        {
            if ((DateTime.Now > oldModule.EndDate) && (DateTime.Now > newModule.EndDate))
                return;                             // No logging of changes in the passed

            Notification notification = new Notification();
            notification.CourseId = newModule.CourseId;
            notification.ChangeTime = DateTime.Now;
            notification.EndOfRelevance = newModule.EndDate;

            string changeText = "En modul är förändrad: \n";
            bool changeConfirmed = false;
            if (oldModule.Name != newModule.Name)
            {
                changeConfirmed = true;
                changeText += "Modulens namn är ändrat från " + oldModule.Name + " till " + newModule.Name + "\n";
            }
            if (oldModule.StartDate != newModule.StartDate)
            {
                changeConfirmed = true;
                changeText += "Modulens start är ändrat från " + oldModule.StartDate.ToString() +
                    " till " + newModule.StartDate.ToString() + "\n";
            }
            if (oldModule.EndDate != newModule.EndDate)
            {
                changeConfirmed = true;
                changeText += "Modulens slut är ändrat från " + oldModule.EndDate.ToString() +
                    " till " + newModule.EndDate.ToString() + "\n";
            }
            if (oldModule.Description != newModule.Description)
            {
                changeConfirmed = true;
                changeText += "Modulens beskrivning är ändrad\n";
            }
            if (changeConfirmed)
            {
                notification.ChangeText = changeText;
                db.Notifications.Add(notification);
                db.SaveChanges();
                // CALL A STUDENT REPO METHOD TO UPDATE ALL STUDENTS OF THE COURSE WITH THIS ITEM TO myNotes
            }
        }

        // ADD a notification about a new activity
        public static void AddNewActivityNote(Activity activity)
        {
            if (DateTime.Now > activity.EndDate)
                return;                             // No logging of changes in the passed

            Notification notification = new Notification();
            int courseId = ModuleRepo.RetrieveModuleCourseId(activity.ModuleId);
            if (courseId < 1)
                return;             // Bad request for courseId
            notification.CourseId = courseId;
            notification.ChangeTime = DateTime.Now;
            notification.EndOfRelevance = activity.EndDate;

            string changeText = "En ny aktivitet är tillagd: " + activity.Name + "\n";
            changeText += "Start: " + activity.StartDate.ToString() + "\n";
            changeText += "Slut: " + activity.EndDate.ToString() + "\n";
            changeText += "Typ: " + ((SelectActivityType)activity.ActivityType).ToString() + "\n";
            notification.ChangeText = changeText;

            db.Notifications.Add(notification);
            db.SaveChanges();

            // CALL A STUDENT REPO METHOD TO UPDATE ALL STUDENTS OF THE COURSE WITH THIS ITEM TO myNotes
        }

        // ADD a notification about a removed activity
        public static void AddRemovedActivityNote(Activity activity)
        {
            if (DateTime.Now > activity.EndDate)
                return;                             // No logging of changes in the passed

            Notification notification = new Notification();
            int courseId = ModuleRepo.RetrieveModuleCourseId(activity.ModuleId);
            if (courseId < 1)
                return;             // Bad request for courseId
            notification.CourseId = courseId;
            notification.ChangeTime = DateTime.Now;
            notification.EndOfRelevance = activity.EndDate;

            string changeText = "En aktivitet är borttagen: " + activity.Name + "\n";
            changeText += "Start: " + activity.StartDate.ToString() + "\n";
            changeText += "Slut: " + activity.EndDate.ToString() + "\n";
            changeText += "Typ: " + ((SelectActivityType)activity.ActivityType).ToString() + "\n";
            notification.ChangeText = changeText;

            db.Notifications.Add(notification);
            db.SaveChanges();

            // CALL A STUDENT REPO METHOD TO UPDATE ALL STUDENTS OF THE COURSE WITH THIS ITEM TO myNotes
        }

        // ADD a notification about a changed activity
        public static void AddChangedActivityNote(Activity oldActivity, Activity newActivity)
        {
            if ((DateTime.Now > oldActivity.EndDate) && (DateTime.Now > newActivity.EndDate))
                return;                             // No logging of changes in the passed

            Notification notification = new Notification();
            int courseId = ModuleRepo.RetrieveModuleCourseId(newActivity.ModuleId);
            if (courseId < 1)
                return;             // Bad request for courseId
            notification.CourseId = courseId;
            notification.ChangeTime = DateTime.Now;
            notification.EndOfRelevance = newActivity.EndDate;

            string changeText = "En aktivitet är förändrad: \n";
            bool changeConfirmed = false;
            if (oldActivity.Name != newActivity.Name)
            {
                changeConfirmed = true;
                changeText += "Aktivitetens namn är ändrat från " + oldActivity.Name + " till " + newActivity.Name + "\n";
            }
            if (oldActivity.StartDate != newActivity.StartDate)
            {
                changeConfirmed = true;
                changeText += "Aktivitetens start är ändrat från " + oldActivity.StartDate.ToString() + 
                    " till " + newActivity.StartDate.ToString() + "\n";
            }
            if (oldActivity.EndDate != newActivity.EndDate)
            {
                changeConfirmed = true;
                changeText += "Aktivitetens slut är ändrat från " + oldActivity.EndDate.ToString() +
                    " till " + newActivity.EndDate.ToString() + "\n";
            }
            if (oldActivity.ActivityType != newActivity.ActivityType)
            {
                changeConfirmed = true;
                changeText += "Aktivitetens typ är ändrat från " + ((SelectActivityType)oldActivity.ActivityType).ToString() +
                    " till " + ((SelectActivityType)newActivity.ActivityType).ToString() + "\n";
            }
            if (oldActivity.Description != newActivity.Description)
            {
                changeConfirmed = true;
                changeText += "Aktivitetens beskrivning är ändrad\n"; 
            }
            if (changeConfirmed)
            {
                notification.ChangeText = changeText;
                db.Notifications.Add(notification);
                db.SaveChanges();
                // CALL A STUDENT REPO METHOD TO UPDATE ALL STUDENTS OF THE COURSE WITH THIS ITEM TO myNotes
            }
        }

    }
}