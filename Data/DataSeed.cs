using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using POS_GG_APP.Models;
using System;
using System.Threading.Tasks;

namespace POS_GG_APP.Data
{
    public static class DataSeed
    {
        public static async Task Seed(IHost app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    await roleManager.CreateIfNotExistAsync(Roles.Administrator);
                    await roleManager.CreateIfNotExistAsync(Roles.Manager);
                    await roleManager.CreateIfNotExistAsync(Roles.Warehouse);
                    await roleManager.CreateIfNotExistAsync(Roles.Production);
                    await roleManager.CreateIfNotExistAsync(Roles.Kitchen);

                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var configuration = services.GetRequiredService<IConfiguration>();
                    var logger = services.GetRequiredService<ILogger<Program>>();

                    var adminId = Environment.GetEnvironmentVariable("admin_id");
                    var adminPassword = Environment.GetEnvironmentVariable("admin_pw");
                    var adminName = Environment.GetEnvironmentVariable("admin_name");


                    if (string.IsNullOrEmpty(adminId) || string.IsNullOrEmpty(adminPassword))
                    {
                        logger.LogError("Admin email or password is not configured.");
                        throw new Exception("Admin email or password is not configured.");
                    }

                    var adminIdint = int.Parse(adminId);

                    if (adminIdint == 0)
                        throw new ArgumentException("Admin's company Id can't be 0");

                    var adminUser = await userManager.Users.SingleOrDefaultAsync(u => u.CompanyId == adminIdint);

                    if (adminUser != null)
                        return;

                    var user = new ApplicationUser
                    {
                        CompanyId = adminIdint,
                        UserName = adminName,
                    };

                    var result = await userManager.CreateAsync(user, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, Roles.Administrator);
                    }
                    else
                    {
                        throw new Exception("Could not create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while creating the admin user.");
                }
            }
        }

    }
}
