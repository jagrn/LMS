using LMS.Models;
using LMS.Repositories;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LMS.Controllers
{
    public class AdminUsersController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private ApplicationDbContext db = new ApplicationDbContext();
        //private AllCoursesList allCourses = CourseRepo.RetrieveCourseList();

        private List<SelectListItem> GetCoursesList()
        {
            IQueryable<SelectListItem> allCourses;
            allCourses = db.Courses
                  .Select(s => new SelectListItem
                  {
                      Value = s.Id.ToString(),
                      Text = s.Name
                  });


            return allCourses.ToList();
        }

        // GET: AdminUsers
        public ActionResult Index(string strCourseId, string userType, string sortOrder, string searchString)
        {

            //IQueryable<SelectListItem> allCourses;
            //allCourses = db.Courses
            //      .Select(s => new SelectListItem
            //      {
            //          Value = s.Id.ToString(),
            //          Text = s.Name
            //      });

            //SelectList coursesList = new SelectList(allCourses);


            AllCoursesList allCoursesList = CourseRepo.RetrieveCourseList();


            IQueryable<EditUserViewModel> query;
            List<EditUserViewModel> resultList;
            int thisCourseId = 0;

            //{
            //    get
            ////{
            //ApplicationDbContext db = new ApplicationDbContext();
            //Course c = db.Courses.Find(this.CourseId);
            //return c.Name;
            //    }
            //}



            if (userType != null && userType.Equals("Teacher"))
            {
                //Lista lärare
                ViewBag.Title = "Lista lärare";
                query = from u in db.Users
                        where u.CourseId == null
                        select new EditUserViewModel()
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            CourseId = u.CourseId,
                            //CourseName = "",
                            UserRole = "Lärare"

                                };

            }
            else if (userType != null && userType.Equals("Student"))
            {
                //Lista elever
                ViewBag.Title = "Lista elever";
                //string cName = CourseRepo.RetrieveCourseName(int.Parse(strCourseId));
                if (int.TryParse(strCourseId, out thisCourseId))
                {
                    query = from u in db.Users
                            where u.CourseId.Equals(thisCourseId)
                            select new EditUserViewModel()
                            {
                                Id = u.Id,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                Email = u.Email,
                                CourseId = u.CourseId,
                                //CourseName = cName, CourseRepo.RetrieveCourseName(u.CourseId), // (u.CourseId != null ? CourseRepo.RetrieveCourseName(u.CourseId) : ""),
                                UserRole = "Elev"

                            };

                }
                else
                {
                    query = from u in db.Users
                            where u.CourseId != null
                            select new EditUserViewModel()
                            {
                                Id = u.Id,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                Email = u.Email,
                                CourseId = u.CourseId,
                                //CourseName = // (u.CourseId != null ? CourseRepo.RetrieveCourseName(u.CourseId) : ""),
                                UserRole = "Elev"

                            };

                }

            }
            else
            {
                ViewBag.Title = "Inget resultat att lista";
                query = Enumerable.Empty<EditUserViewModel>().AsQueryable();
            }

            switch (sortOrder)
            {
                case "firstname_desc":
                    query = query.OrderByDescending(s => s.FirstName);
                    break;
                case "lastname_desc":
                    query = query.OrderByDescending(s => s.LastName);
                    break;
                case "email_desc":
                    query = query.OrderByDescending(s => s.Email);
                    break;
                default:
                    query = query.OrderBy(s => s.Email);
                    break;
            }



            if (searchString != null && searchString != "")
            {
                query = query.Where(u => u.FirstName.Contains(searchString) || u.LastName.Contains(searchString) || u.Email.Contains(searchString));
            }

           
            resultList = query.ToList();
            return View(resultList.ToList());

            
        }

        // GET: ListStudents
        public ActionResult ListStudents(string sortOrder, string searchString, int? SelectedCourseId)
        {

            IQueryable<EditUserViewModel> query;
            List<EditUserViewModel> resultList;
            //int thisCourseId = 0;

            ViewBag.AllCourses = GetCoursesList();

            //if (!User.IsInRole("Teacher"))
            //{
            //    ApplicationUser user = db.Users.Find()
            //}
            //Lista elever


            //string cName = CourseRepo.RetrieveCourseName(int.Parse(strCourseId));
            //if (int.TryParse(strCourseId, out thisCourseId))
            if (SelectedCourseId > 0)
            {
                query = from u in db.Users
                        where u.CourseId == SelectedCourseId
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

            }
            else
            {
                query = from u in db.Users
                        where u.CourseId != null
                        select new EditUserViewModel()
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            CourseId = u.CourseId,
                            //CourseName =  sätts i EditUserViewModel
                            UserRole = "Elev"

                        };

            }


            //Toggle sortorder
            ViewBag.SortLastName = sortOrder == "LastName_desc" ? "LastName" : "LastName_desc";
            ViewBag.SortFirstName = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
            ViewBag.SortEmail = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewBag.SortCourse= sortOrder == "CourseName" ? "CourseName_desc" : "CourseName";

            switch (sortOrder)
            {
                case "LastName":
                    query = query.OrderBy(s => s.LastName);
                    break;
                case "LastName_desc":
                    query = query.OrderByDescending(s => s.LastName);
                    break;
                case "FirstName":
                    query = query.OrderBy(s => s.FirstName);
                    break;
                case "FirstName_desc":
                    query = query.OrderByDescending(s => s.FirstName);
                    break;
                case "Email":
                    query = query.OrderBy(s => s.Email);
                    break;
                case "Email_desc":
                    query = query.OrderByDescending(s => s.Email);
                    break;
                //case "CourseName":
                //    query = query.OrderBy(s => s.CourseName);
                //    break;
                //case "CourseName_desc":
                //    query = query.OrderByDescending(s => s.CourseName);
                //    break;
                default:
                    query = query.OrderBy(s => s.LastName);
                    break;
            }



            if (searchString != null && searchString != "")
            {
                query = query.Where(u => u.FirstName.Contains(searchString) || u.LastName.Contains(searchString) || u.Email.Contains(searchString));
                //query = query.Where(u => u.FirstName.Contains(searchString) || u.LastName.Contains(searchString) || u.Email.Contains(searchString) || u.CourseName.Contains(searchString));
            }

            
            resultList = query.ToList();
            if (resultList.Count > 0)
            {
                ViewBag.SearchResult = string.Format("Antal sökträffar {0}", query.Count());
            }
            else
            {
                ViewBag.SearchResult = "Inga sökträffar";
            }
               
            return View(resultList.ToList());


        }

        // GET: ListTeachers
        public ActionResult ListTeachers(string sortOrder, string searchString)
        {

            IQueryable<EditUserViewModel> query;
            List<EditUserViewModel> resultList;

                //Lista lärare

                query = from u in db.Users
                        where u.CourseId == null
                        select new EditUserViewModel()
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            CourseId = u.CourseId,
                            UserRole = "Lärare"

                        };


            //Toggle sortorder
            ViewBag.SortLastName = sortOrder == "LastName_desc" ? "LastName" : "LastName_desc";
            ViewBag.SortFirstName = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
            ViewBag.SortEmail = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewBag.SortCourse = sortOrder == "CourseName" ? "CourseName_desc" : "CourseName";

            switch (sortOrder)
            {
                case "LastName":
                    query = query.OrderBy(s => s.LastName);
                    break;
                case "LastName_desc":
                    query = query.OrderByDescending(s => s.LastName);
                    break;
                case "FirstName":
                    query = query.OrderBy(s => s.FirstName);
                    break;
                case "FirstName_desc":
                    query = query.OrderByDescending(s => s.FirstName);
                    break;
                case "Email":
                    query = query.OrderBy(s => s.Email);
                    break;
                case "Email_desc":
                    query = query.OrderByDescending(s => s.Email);
                    break;
                //case "CourseName":
                //    query = query.OrderBy(s => s.CourseName);
                //    break;
                //case "CourseName_desc":
                //    query = query.OrderByDescending(s => s.CourseName);
                //    break;
                default:
                    query = query.OrderBy(s => s.LastName);
                    break;
            }



            if (searchString != null && searchString != "")
            {
                query = query.Where(u => u.FirstName.Contains(searchString) || u.LastName.Contains(searchString) || u.Email.Contains(searchString));
            }



            resultList = query.ToList();
            if (resultList.Count > 0)
            {
                ViewBag.SearchResult = string.Format("Antal sökträffar {0}", query.Count());
            }
            else
            {
                ViewBag.SearchResult = "Inga sökträffar";
            }
            return View(resultList.ToList());


        }


        // GET: AdminUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: AdminUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,CourseId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(applicationUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicationUser);
        }

        // GET: AdminUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: AdminUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,CourseId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        public async Task<ActionResult> Delete(string id)
        {
            ViewBag.ErrorDescription = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.AllCourses = GetCoursesList();

            ApplicationUser u = db.Users.Find(id);
            if (u == null)
            {
                return HttpNotFound();
            }

            var roles = await UserManager.GetRolesAsync(id);
            var role = roles.FirstOrDefault();

            var model = new EditUserViewModel()
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                CourseId = u.CourseId,
                UserRole = role
            };


            return View(model);

        }
        // GET: AdminUsers/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ApplicationUser applicationUser = db.Users.Find(id);
        //    if (applicationUser == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(applicationUser);
        //}

        // POST: AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);

            try
            {

                db.Users.Remove(applicationUser);
                db.SaveChanges();
                if (applicationUser.CourseId != null && applicationUser.CourseId > 0)
                {
                    return RedirectToAction("ListStudents");
                }
                else
                {
                    return RedirectToAction("ListTeachers");
                }

            }
            catch
            {
                var model = new EditUserViewModel();
                model.Email = applicationUser.Email;
                model.CourseId = applicationUser.CourseId;
                ViewBag.ErrorDescription = string.Format("Det gick inte att ta bort användaren {0}.", applicationUser.Email);
                return View(model);
            }

        }

        // GET: AdminUsers/Create
        public ActionResult CreateNewUser(int? courseId, string userType)
        {
            var model = new RegisterViewModel();

            model.CourseId = courseId;

            userType = userType == null ? "" : userType;

            if (courseId == null && userType == "")
            {
                model.UserRole = "Student";
            }
            else if (courseId == null && userType == "Teacher")
            {
                model.UserRole = "Teacher";
            }
            else
            {
                model.UserRole = "Student";
            }
               
            ViewBag.AllCourses = GetCoursesList();
            return View(model);

        }


        public ActionResult ListEnrolledStudents(RegisterViewModel model)
        {
            TestController tc = new TestController();

            return tc.GetEnrolledStudents(model.CourseId == null ? "0" : model.CourseId.ToString());
          

        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNewUser(RegisterViewModel model)
        {


            if (ModelState.IsValid && (model.UserRole == "Teacher" || model.UserRole == "Student"))
            {
                var user = new ApplicationUser { FirstName = model.FirstName, LastName = model.LastName, UserName = model.Email, Email = model.Email, CourseId = model.CourseId };
                if (!db.Users.Any(u => u.UserName == model.Email))
                {
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {

                        //addToRole
                         result = await UserManager.AddToRoleAsync(user.Id, model.UserRole);
                        ViewBag.SuccessDesc = string.Format("Användaren {0} är sparad", user.Email);
                        return RedirectToAction("CreateNewUser", "AdminUsers", new { courseId = model.CourseId, userType = model.UserRole });
                    }
                }
                else
                {
                    ViewBag.ErrorDesc = "En användaren finns redan";
                }
            }
            else
            {
                ViewBag.ErrorDesc = "Kunde ej skapa ny användare";
            }

            ViewBag.ErrorDesc = "Kunde ej skapa ny användare";
            ViewBag.AllCourses = GetCoursesList();
            return View(model);
        }

        // GET: AdminUsers/Create
        public ActionResult CreateUser(int? courseId)
        {
            var model = new RegisterViewModel();

            if (courseId > 0)
            {
                model.CourseId = courseId;
            }
     
            return View(model);

        }
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(RegisterViewModel model)
        {
            
            if (ModelState.IsValid)
            {


                //if (model.CourseId == null)
                //{
                //    model.CourseId = 0;
                //}

                var user = new ApplicationUser { FirstName = model.FirstName, LastName = model.LastName, UserName = model.Email, Email = model.Email, CourseId = model.CourseId };
                if (!db.Users.Any(u => u.UserName == model.Email))
                {
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                        //addToRole
                        string role = "Student";
                        if (model.CourseId == null)
                        {
                            role = "Teacher";
                        }
                        result = await UserManager.AddToRoleAsync(user.Id, role);

                        //int? courseId = model.CourseId;

                        //var emptyModel = new RegisterViewModel();

                        //if (courseId > 0)
                        //{
                        //    emptyModel.CourseId = courseId;
                        //}
                        return RedirectToAction("CreateUser", "AdminUsers", new { courseId = model.CourseId});
                     

                        
                    }

                }
                else
                {
                    //Användarnamn upptaget
                }

                //AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult _ListUsers(string sortOrder, string searchString)

        {
            //searchString = ""; // "teacher5";
            IQueryable<EditUserViewModel> query;
            List<EditUserViewModel> resultList;

            if (searchString != null &&  searchString != "")
            {
                query = from u in db.Users
                            where u.LastName.Contains(searchString)
                                    || u.FirstName.Contains(searchString)
                                    || u.Email.Contains(searchString)
                            select new EditUserViewModel()
                            {
                                Id = u.Id,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                Email = u.Email,
                                CourseId = u.CourseId,
                                UserRole = ""
             
                              
                            };


     
            }
            else
            {
                query = from u in db.Users

                        select new EditUserViewModel()
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            CourseId = u.CourseId,
                            UserRole = ""

        };

               
            }

       

            switch (sortOrder)
            {
                case "firstname_desc":
                    query = query.OrderByDescending(s => s.FirstName);
                    break;
                case "lastname_desc":
                    query = query.OrderByDescending(s => s.LastName);
                    break;
                case "email_desc":
                    query = query.OrderByDescending(s => s.Email);
                    break;
                default:
                    query = query.OrderBy(s => s.Email);
                    break;
            }

            resultList = query.ToList();
            return PartialView(resultList.ToList());




            //if (searchString != "")
            //{
            //     result = db.Users.Where(u => u.LastName.Contains(searchString)
            //                   || u.FirstName.Contains(searchString)
            //                   || u.Email.Contains(searchString));
            //    ViewBag.Searched = "eftersomboolfunkarej";
            //    if (!result.Any())
            //    {
            //        if (searchString != "")
            //        {
            //            ViewBag.Description = "Kunde inte hitta resultat som innehåller: " + searchString;
            //        }
            //        else
            //        {
            //            ViewBag.Description = "Vänligen ange ett sökkriterie";
            //        }
            //        return PartialView(result?.ToList());
            //    }

            //    return PartialView(result.ToList());
            //}
            //else
            //{

            //    var query = from u in db.Users
            //                select new RegisterViewModel()
            //                {
            //                    FirstName = u.FirstName,
            //                    LastName = u.LastName,
            //                    Email = u.Email,
            //                    CourseId = u.CourseId,
            //                    UserRole = "Lärare"
            //                   // UserRoles = u.Roles.ToList();
                                

            //                }
                            
            //                ;

            //    usersList = query.ToList();
            //    result = usersList;
         
               
            //}

   

            //return PartialView(usersList.ToList());


        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public async Task<ActionResult> Register(string id)
        {
            if (id == null)
            {
                return View();
            }
       

            ApplicationUser u = db.Users.Find(id);
            if (u == null)
            {
                return HttpNotFound();
            }
            var roles = await UserManager.GetRolesAsync(id);
            var role = roles.FirstOrDefault();

            var model = new RegisterViewModel()
            {
   
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                CourseId = u.CourseId,
                UserRole = role
            };
            
           
            return View(model);

        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
 
            if (ModelState.IsValid)
            {

                //var roleStore = new RoleStore<IdentityRole>(context);
                //var roleManager = new RoleManager<IdentityRole>(roleStore);

                //var roleNames = new[] { "Admin", "Teacher", "Student" };
                //var userNames = new[] { "admin@lms.se", "teacher@lms.se", "student@lms.se" };



                //foreach (var roleName in roleNames)
                //{
                //    if (!context.Roles.Any(r => r.Name == roleName))
                //    {
                //        var role = new IdentityRole { Name = roleName };
                //        var result = roleManager.Create(role);
                //        if (!result.Succeeded)
                //        {
                //            throw new Exception(string.Join("/n", result.Errors));
                //        }
                //    }

                //}

                //var userStore = new UserStore<ApplicationUser>(context);
                //var userManager = new UserManager<ApplicationUser>(userStore);

                //for (int i = 0; i < userNames.Length; i++)
                //{
                //    var userName = userNames[i];
                //    var roleName = roleNames[i];
                //    if (!context.Users.Any(u => u.UserName == userName))
                //    {
                //        var user = new ApplicationUser
                //        {
                //            UserName = userName,
                //            Email = userName
                //        };
                //        var result = userManager.Create(user, "123456");
                //        if (!result.Succeeded)
                //        {
                //            throw new Exception(string.Join("/n", result.Errors));
                //        }
                //        userManager.AddToRole(user.Id, roleName);
                //        //userManager.AddToRole(userManager.FindByName(userNames[i]).Id, roleNames[i]);
                //    }

                //}







                //Student = 1
                //int RoleType = 1;

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                if (!db.Users.Any(u => u.UserName == model.Email))
                {
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                        //addToRole
                        result = await UserManager.AddToRoleAsync(user.Id, "Teacher");


                        return RedirectToAction("Register", "AdminUsers");
                    }

                }
                else
                {
                    //Användarnamn upptaget
                }
                
                //AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: AdminUsers/Edit/5
        public async Task<ActionResult> EditUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.AllCourses = GetCoursesList();

            ApplicationUser u = db.Users.Find(id);
            if (u == null)
            {
                return HttpNotFound();
            }

            var roles = await UserManager.GetRolesAsync(id);
            var role = roles.FirstOrDefault();

            var model = new EditUserViewModel()
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                CourseId = u.CourseId,
                UserRole = role
            };


            return View(model);

        }
        // POST: AdminUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        //public Task<ActionResult> EditUser([Bind(Include = "Id,FirstName,LastName,CourseId,Email")] EditUserViewModel currentUser)
        public ActionResult EditUser([Bind(Include = "Id,FirstName,LastName,CourseId,Email,UserRole")] EditUserViewModel currentUser)


        {
            ViewBag.AllCourses = GetCoursesList();

            if (ModelState.IsValid)
            {

                //if (currentUser.CourseId == null)
                //{
                //    currentUser.CourseId = 0;
                //}

                //if (ModelState.IsValid)
                //{
                //    db.Entry(applicationUser).State = EntityState.Modified;
                //    db.SaveChanges();
                //    return RedirectToAction("Index");
                //}




                //ApplicationUser applicationUser = StudentRepo.GetUser(currentUser.Id);
                try
                {
                        ApplicationUser applicationUser = db.Users.Find(currentUser.Id);


                        applicationUser.Id = currentUser.Id;
                        applicationUser.FirstName = currentUser.FirstName;
                        applicationUser.LastName = currentUser.LastName;
                        applicationUser.Email = currentUser.Email;
                        applicationUser.CourseId = currentUser.CourseId;
                
                        db.Entry(applicationUser).State = EntityState.Modified;

                        db.SaveChanges();
                    ViewBag.SuccessMsg = "Ändringarna har sparats."; 


                }
                catch (System.Exception ex)
                {

                    ViewBag.ErrorDescription = ex.Message;
                }
                return View(currentUser); 
            }
            ViewBag.ErrorDescription = "Ändringarna kunde inte sparas";
            return View(currentUser);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
