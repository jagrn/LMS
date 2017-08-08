namespace LMS.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LMS.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(LMS.Models.ApplicationDbContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var roleNames = new[] { "Admin", "Teacher", "Student" };
            var userNames = new[] { "admin@lms.se", "teacher@lms.se", "student@lms.se" };



            foreach (var roleName in roleNames)
            {
                if (!context.Roles.Any(r => r.Name == roleName))
                {
                    var role = new IdentityRole { Name = roleName };
                    var result = roleManager.Create(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception(string.Join("/n", result.Errors));
                    }
                }

            }

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            for (int i = 0; i < userNames.Length; i++)
            {
                var userName = userNames[i];
                var roleName = roleNames[i];
                if (!context.Users.Any(u => u.UserName == userName))
                {
                    var user = new ApplicationUser
                    {
                        UserName = userName,
                        Email = userName
                    };
                    var result = userManager.Create(user, "123456");
                    if (!result.Succeeded)
                    {
                        throw new Exception(string.Join("/n", result.Errors));
                    }
                    userManager.AddToRole(user.Id, roleName);
                    //userManager.AddToRole(userManager.FindByName(userNames[i]).Id, roleNames[i]);
                }

            }

            /*
                        foreach (var userName in userNames)
                        {
                            if (!context.Users.Any(u => u.UserName == userName))
                            {
                                var user = new ApplicationUser
                                {
                                    UserName = userName + "@LMS.se",
                                    Email = userName + "@LMS.se"
                                };
                                var result = userManager.Create(user,"123456");
                                if (!result.Succeeded)
                                {
                                    throw new Exception(string.Join("/n", result.Errors));
                                }
                            }

                        }
                        userManager.AddToRole(userManager.FindByName("admin").Id, "Admin");
                        userManager.AddToRole(userManager.FindByName("teacher").Id, "Teacher");
                        userManager.AddToRole(userManager.FindByName("student").Id, "Student");
            */

        }
    }
}
