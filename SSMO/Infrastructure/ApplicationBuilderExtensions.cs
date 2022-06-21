namespace SSMO.Infrastructure
{
    using System;
    using System.Linq;
    using SSMO.Data;
    using SSMO.Data.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;


    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase(
            this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var services = serviceScope.ServiceProvider;

            MigrateDatabase(services);

            SeedCurrency(services);
            SeedDescriptionProducts(services);
            SeedGrades(services);
            SeedSizes(services);
          //  SeedAdministrator(services);

            return app;
        }

        private static void MigrateDatabase(IServiceProvider services)
        {
            var data = services.GetRequiredService<ApplicationDbContext>();

            data.Database.Migrate();
        }

        private static void SeedCurrency(IServiceProvider services)
        {
            var data = services.GetRequiredService<ApplicationDbContext>();

            if (data.Currencys.Any())
            {
                return;
            }

            data.Currencys.AddRange(new[]
            {
              new Currency { Name = "EUR" },
              new Currency { Name = "BGN" },
              new Currency { Name = "USD" },
              
            });

            data.SaveChanges();
        }


        public static void SeedDescriptionProducts(IServiceProvider service)
        {
            var data = service.GetRequiredService<ApplicationDbContext>();

            if (data.Descriptions.Any())
            {
                return;
            }

            data.Descriptions.AddRange(
                new[]
                {
                    new Description {Name = "Birch Film Faced Plywood"},
                    new Description {Name = "Birch Plywood"},
                    new Description {Name = "Poplar Film Faced Plywood"},
                    new Description {Name = "Poplar Plywood"},
                    new Description {Name = "Pine Film Faced Plywood"},
                    new Description {Name = "Pine Plywood"},
                    new Description {Name = "Twin Film Faced Plywood"},
                    new Description {Name = "Twin Film Faced Plywood/Birch & Pine"},
                    new Description {Name = "Twin Film Faced Plywood/Poplar & Pine"},
                    new Description {Name = "Combi Film Faced Plywood"},
                    new Description {Name = "Combi Film Faced Plywood/Birch & Pine"},
                    new Description {Name = "Combi Film Faced Plywood/Poplar & Pine"}
                }
                );

            data.SaveChanges();
        }

        private static void SeedGrades(IServiceProvider service)
        {
            var data = service.GetRequiredService<ApplicationDbContext>();

            if (data.Grades.Any())
            {
                return;
            }

            data.Grades.AddRange(new[]
            {
                new Grade{Name = "A"},
                new Grade{Name = "B"},
                new Grade {Name = "C"},
                new Grade {Name = "I/I"},
                new Grade {Name = "I/II"},
                new Grade {Name = "II/II"},
                new Grade {Name ="II/III"},
                new Grade {Name ="II/IV"},
                new Grade{Name ="III/III"},
                new Grade {Name="III/IV"},
                new Grade{Name ="B/BB"},
                new Grade{Name="BB/BB"},
                new Grade {Name ="BB/CP"},
                new Grade {Name ="BB/C"},
                new Grade {Name ="CP/CP"},
                new Grade {Name ="CP/C"},
                new Grade {Name ="C/C"}
            });

            data.SaveChanges();

        }

     
        private static void SeedSizes(IServiceProvider service)
        {
            var data = service.GetRequiredService<ApplicationDbContext>();

            if (data.Sizes.Any())
            {
                return;
            }

            data.Sizes.AddRange(new[]
            {
                new Size {Name = "3/1250/2500"},
                new Size {Name ="3/2500/1250"},
                new Size {Name = "3/1220/2440"},
                new Size {Name = "3/2440/1220"},
                new Size {Name ="4/1250/2500"},
                new Size {Name = "4/2500/1250"},
                new Size {Name = "4/1220/2440"},
                new Size {Name = "4/2440/1220"},
                new Size {Name = "6/1250/2500"},
                new Size {Name = "6/2500/1250"},
                new Size {Name = "6/1220/2440"},
                new Size {Name = "6/2440/1220"},
                new Size {Name = "9/1250/2500"},
                new Size {Name = "9/2500/1250"},
                new Size {Name = "9/1220/2440"},
                new Size {Name = "9/2440/1220"},
                new Size {Name = "10/1250/2500"},
                new Size {Name = "10/2500/1250"},
                new Size {Name = "10/1220/2440"},
                new Size {Name ="10/2440/1220"},
                new Size {Name = "12/1250/2500"},
                new Size {Name = "12/2500/1250"},
                new Size {Name ="12/1220/2440"},
                new Size {Name = "12/2440/1220"},
                new Size {Name = "15/1250/2500"},
                new Size {Name = "15/2500/1250"},
                new Size {Name = "15/1220/2440"},
                new Size {Name = "15/2440/1220"},
                new Size {Name = "18/1250/2500"},
                new Size {Name = "18/2500/1250"},
                new Size {Name = "18/1220/2440"},
                new Size {Name = "18/2440/1220"},
                new Size {Name = "18/1500/3000"},
                new Size {Name = "21/1250/2500"},
                new Size {Name = "21/2500/1250"},
                new Size {Name = "21/1220/2440"},
                new Size {Name = "21/2440/1220"},
                new Size {Name ="24/1250/2500"},
                new Size {Name = "24/2500/1250"},
                new Size {Name = "24/1220/2440"},
                new Size {Name ="24/2440/1220"},
                new Size {Name = "27/1250/2500"},
                new Size {Name = "27/2500/1250"},
                new Size {Name = "27/1220/2440"},
                new Size {Name = "27/2440/1220"},
                new Size {Name = "30/1250/2500"},
                new Size {Name = "30/2500/1250"},
                new Size {Name = "30/1220/2440"},
                new Size {Name = "30/2440/1220"},
            });

            data.SaveChanges();
        }

        

        //    private static void SeedAdministrator(IServiceProvider services)
        //    {
        //        var userManager = services.GetRequiredService<UserManager<User>>();
        //        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        //        Task
        //            .Run(async () =>
        //            {
        //                if (await roleManager.RoleExistsAsync(AdministratorRoleName))
        //                {
        //                    return;
        //                }

        //                var role = new IdentityRole { Name = AdministratorRoleName };

        //                await roleManager.CreateAsync(role);

        //                const string adminEmail = "admin@crs.com";
        //                const string adminPassword = "admin12";

        //                var user = new User
        //                {
        //                    Email = adminEmail,
        //                    UserName = adminEmail,
        //                    FullName = "Admin"
        //                };

        //                await userManager.CreateAsync(user, adminPassword);

        //                await userManager.AddToRoleAsync(user, role.Name);
        //            })
        //            .GetAwaiter()
        //            .GetResult();
        //    }

    }
}
