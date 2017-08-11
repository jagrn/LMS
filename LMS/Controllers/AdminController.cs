using LMS.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LMS.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        // GET: Admin
        public ActionResult Index()
        {
            //var context = new ApplicationDbContext();
            //var userStore = new UserStore<ApplicationUser>(context);
            //var userManager = new UserManager<ApplicationUser>(userStore);



            //return View(userStore.Users.ToList());

            return View(db.Users.ToList());
        }

        public ActionResult ListUsers(string id)
        {
            //foreach (var user in db.Users.ToList())
            //{
            //    var u = new LMSUsers();
            //    u.Email = user.Email:
            //    u.Id = user.Id;
            //    u
            //};
            if (id == null)
            {
                return View(db.Users.ToList());
            }
            else
            {
                //edit
                return View(db.Users.ToList());
            }
               
        }


        // GET: Admin/Details/5
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
           // return View();
        }

        // GET: Admin/Create
        public ActionResult Create(string id)
        {

            if (id == null)
            {
                return View();
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return View();
            }
            return View(applicationUser);

          }

        public ActionResult _EditUser(string id)
        {
            //List<employee> employee = new List<employee>();
            //int _desigId = 0;
            //int _deptId = 0;
            //Int32.TryParse(desigId, out _desigId);
            //Int32.TryParse(deptId, out _deptId);

            //employee = db.Employees.Where(p => (p.DeptId == _deptId || _deptId == 0) &&
            //    (p.DesignationId == _desigId || _desigId == 0)).ToList();
            //;
            if (id == null)
            {
                return PartialView();
            }
            else
            {
                ApplicationUser applicationUser = db.Users.Find(id);
                return PartialView(applicationUser);
            }
                
        }

        public ActionResult _ListUsers()

        {
            string s = "teacher5";
            if(s != "")
            { 
                var result = db.Users.Where(u => u.Email.Contains(s));
                ViewBag.Searched = "eftersomboolfunkarej";
                if (!result.Any())
                {
                    if (s != "")
                    {
                        ViewBag.Description = "Kunde inte hitta användare med användarnamn: " + s;
                    }
                    else
                    {
                        ViewBag.Description = "Vänligen ange ett användarnamn";
                    }
                    return PartialView(result?.ToList());
                }

                return PartialView(result.ToList());
            }



            //List<employee> employee = new List<employee>();
            //int _desigId = 0;
            //int _deptId = 0;
            //Int32.TryParse(desigId, out _desigId);
            //Int32.TryParse(deptId, out _deptId);

            //employee = db.Employees.Where(p => (p.DeptId == _deptId || _deptId == 0) &&
            //    (p.DesignationId == _desigId || _desigId == 0)).ToList();
            //;
            return PartialView(db.Users.ToList());
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
        // POST: /Account/Create
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    //return RedirectToAction("Index", "Home");
                    return RedirectToAction("ListUsers", "Admin");
                }

                //AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //// POST: Admin/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,FirstName,LastName,CourseId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        var context = new ApplicationDbContext();
        //        var userStore = new UserStore<ApplicationUser>(context);
        //        var userManager = new UserManager<ApplicationUser>(userStore);

        //        string email = applicationUser.Email;

        //            if (!context.Users.Any(u => u.UserName == email))
        //                {
        //                    var user = new ApplicationUser
        //                    {
        //                        UserName = email,
        //                        Email = email,
        //                    };
        //                    var result = userManager.Create(user, email);

        //            if (!result.Succeeded)
        //                    {
        //                        throw new Exception(string.Join("\n", result.Errors));
        //                    }

        //                }


        //            var addedUser = userManager.FindByName(email);
        //            userManager.AddToRole(addedUser.Id, "Teacher");


        //    }

        //    return RedirectToAction("Index");

        //    //if (ModelState.IsValid)
        //    //{
        //    //    db.Users.Add(applicationUser);
        //    //    db.SaveChanges();
        //    //    return RedirectToAction("Index");
        //    //}

        //    return View(applicationUser);
        //    //return View();
        //}

        // GET: Admin/Edit/5
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
           // return View();
        }

        // POST: Admin/Edit/5
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

        // GET: Admin/Delete/5
        public ActionResult Delete(string id)
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
            //return View();
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
            db.SaveChanges();
            return RedirectToAction("Index");
            
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
