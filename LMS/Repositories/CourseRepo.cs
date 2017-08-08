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
    public enum CourseRepoResult
    {
        Success,
        BadRequest,
        NotFound,
    }

    public struct SingleCourse
    {
        public Course course;
        public CourseRepoResult repoResult;
    }

    public struct AllCoursesList
    {
        public List<CourseListData> courseList;
        public CourseRepoResult repoResult;
    }

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
                return "No name retrieved: Undefined course ID";

            var courses = db.Courses.Where(c => c.Id == courseId).ToList();
            if (courses.Count != 1)
            {
                return "No name retrieved: Course ID not found";
            }
            return courses.First().Name;
        }

        // RETREIVE a single course
        public static SingleCourse RetrieveCourse(int? courseId)
        {
            var singleCourse = new SingleCourse();

            if ((courseId == null) || (courseId == 0))
            {
                singleCourse.repoResult = CourseRepoResult.BadRequest;
                return singleCourse;
            }

            var courses = db.Courses.Where(c => c.Id == courseId).ToList();
            if (courses.Count != 1)
            {
                singleCourse.repoResult = CourseRepoResult.NotFound;
                return singleCourse;
            }

            singleCourse.course = courses.First();
            singleCourse.repoResult = CourseRepoResult.Success;
            return singleCourse;
        }

        // RETREIVE a list of courses (id + name)
        public static AllCoursesList RetrieveCourseList()
        {
            var allCoursesList = new AllCoursesList();

            var courses = db.Courses.Where(c => c.Id == c.Id).ToList();
            allCoursesList.courseList = new List<CourseListData>();
            foreach (var cor in courses)
            {
                CourseListData course = new CourseListData();
                course.Id = cor.Id;
                course.Name = cor.Name;
                allCoursesList.courseList.Add(course);
            }
            allCoursesList.repoResult = CourseRepoResult.Success;
            return allCoursesList;
        }

        // ADD a single course
        public static int AddCourse(Course course)
        {
            db.Courses.Add(course);
            db.SaveChanges();
            return course.Id;               // Return newly checked out id to caller      
        }

        // UPDATE a single course
        public static CourseRepoResult UpdateCourse(Course course)
        {
            if (course.Id == 0)
                return CourseRepoResult.BadRequest;

            var courses = db.Courses.Where(c => c.Id == course.Id).ToList();
            if (courses.Count != 1)
                return CourseRepoResult.NotFound;

            courses.First().Name = course.Name;
            courses.First().Description = course.Description;
            courses.First().StartDate = course.StartDate;
            courses.First().EndDate = course.EndDate;

            db.Entry(courses.First()).State = EntityState.Modified;
            db.SaveChanges();
            return CourseRepoResult.Success;
        }

        // REMOVE a single course
        public static CourseRepoResult RemoveCourse(int? courseId)
        {
            if ((courseId == null) || (courseId == 0))
                return CourseRepoResult.BadRequest;

            var courses = db.Courses.Where(c => c.Id == courseId).ToList();
            if (courses.Count != 1)
                return CourseRepoResult.NotFound;

            db.Courses.Remove(courses.First());
            db.SaveChanges();
            return CourseRepoResult.Success;
        }

        // REMOVE all courses
        public static CourseRepoResult RemoveCourses()
        {
            var courses = db.Courses.Where(c => c.Id == c.Id).ToList();
            if (courses.Count == 0)
                return CourseRepoResult.NotFound;

            foreach (var cor in courses)
            {
                db.Courses.Remove(cor);
            }
            db.SaveChanges();
            return CourseRepoResult.Success;
        }
    }
}