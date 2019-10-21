namespace madozdev_blog.Migrations
{
    using madozdev_blog.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<madozdev_blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(madozdev_blog.Models.ApplicationDbContext context)
        {

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });

                //  This method will be called after migrating to the latest version.

                //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
                //  to avoid creating duplicate seed data.
            }

            if(!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if(!context.Users.Any(u => u.Email == "madozc@gmail.com"))
            {
                userManager.Create(new ApplicationUser {
                UserName = "madozc@gmail.com",
                Email = "madozc@gmail.com",
                FirstName = "christophe",
                LastName = "madoz",
                DisplayName = "Student"               
                },"Abc&123");
            }

            if (!context.Users.Any(u => u.Email == "JoeSchmo@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "JoeSchmo@Mailinator.com",
                    Email = "JoeSchmo@Mailinator.com",
                    FirstName = "pascal",
                    LastName = "Mad",
                    DisplayName = "pascal"


                }, "Abc&123!");
            }


            var adminId = userManager.FindByEmail("madozc@gmail.com").Id;
            userManager.AddToRole(adminId, "Admin");


            var moderatorId = userManager.FindByEmail("JoeSchmo@Mailinator.com").Id;
            userManager.AddToRole(moderatorId, "Moderator");
        }
    }
}
