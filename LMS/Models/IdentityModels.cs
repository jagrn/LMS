﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LMS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + LastName; }  }
        public int? CourseId { get; set; }
        public virtual ICollection<StudentNotification> StudentNotifications { get; set; }
        public virtual ICollection<Document> Documents { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<StudentNotification> StudentNotifications { get; set; }
        public DbSet<Document> Documents { get; set; }


        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            var db = new ApplicationDbContext();
            return new ApplicationDbContext();
        }

        //public System.Data.Entity.DbSet<LMS.Models.RegisterViewModel> RegisterViewModels { get; set; }

        //public System.Data.Entity.DbSet<LMS.Models.EditUserViewModel> EditUserViewModels { get; set; }

        //public System.Data.Entity.DbSet<LMS.Models.RegisterViewModel> RegisterViewModels { get; set; }

        //public System.Data.Entity.DbSet<LMS.Models.ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<LMS.Models.ApplicationUser> Users { get; set; }
    }
}