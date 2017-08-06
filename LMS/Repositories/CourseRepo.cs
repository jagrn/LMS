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

    public class CourseRepo
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        // QUERY whether a certain course (id) is present in DB
        public static bool IsCoursePresent(int? courseId)
        {
            if ((courseId == null) || (courseId == 0))
                return false;

            var courses = db.Courses.Where(c => c.Id == courseId).ToList();
            if (courses.Count != 1)
                return false;

            return true;
        }

        // RETREIVE a course name, non-protected method without RepoResult
        public static string RetrieveCourseName(int? courseId)
        {
            if ((courseId == null) || (courseId == 0))
                return "No name retrieved: Undefined module ID";

            var courses = db.Courses.Where(c => c.Id == courseId).ToList();
            if (courses.Count != 1)
            {
                return "No name retrieved: Module ID not found";
            }
            return courses.First().Name;
        }
    }
}