using LMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LMS.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        private ApplicationDbContext db = new ApplicationDbContext();


        public PartialViewResult GetEnrolledStudents(string strCourseId)
        {
            IQueryable<EditUserViewModel> query;
            List<EditUserViewModel> resultList;
            int thisCourseId = 0;

            if (int.TryParse(strCourseId, out thisCourseId))
            {
                ViewBag.CourseName = Repositories.CourseRepo.RetrieveCourseName(thisCourseId);
                
                query = from u in db.Users
                        where u.CourseId == thisCourseId
                        select new EditUserViewModel()
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            CourseId = u.CourseId,
                            //CourseName = sätts i EditUserViewModel
                            UserRole = "Elev"

                        };

            resultList = query.ToList();
            return PartialView("_ListEnrolledStudents", resultList.ToList());

            }

            return PartialView(Enumerable.Empty<EditUserViewModel>().ToList());

        
        }
  
        //public PartialViewResult _ListEnrolledStudents()
        //{
        //    return GetEnrolledStudents();
        //}

     
        //public ActionResult _ListEnrolledStudents(string courseId)
        //{

        //    IQueryable<EditUserViewModel> query;
        //    List<EditUserViewModel> resultList;
        //    int thisCourseId = 1;


        //    //Lista elever


        //    //string cName = CourseRepo.RetrieveCourseName(int.Parse(strCourseId));
        //    if (int.TryParse(courseId, out thisCourseId))
        //    {
        //        query = from u in db.Users
        //                where u.CourseId.Equals(thisCourseId)
        //                select new EditUserViewModel()
        //                {
        //                    Id = u.Id,
        //                    FirstName = u.FirstName,
        //                    LastName = u.LastName,
        //                    Email = u.Email,
        //                    CourseId = u.CourseId,
        //                    //CourseName = sätts i EditUserViewModel
        //                    UserRole = "Elev"

        //                };

        //        resultList = query.ToList();
        //        return PartialView(resultList.ToList());

        //    }
        //    else
        //    {
        //        return PartialView(Enumerable.Empty<EditUserViewModel>());
        //    }





        //}
    }
}