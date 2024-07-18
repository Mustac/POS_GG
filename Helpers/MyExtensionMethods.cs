using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using POS_GG_APP.Models;

namespace POS_GG_APP.Data
{
    public static class MyExtensionMethods
    {
        public static string GetName<T>(this T value) where T : Enum => Enum.GetName(typeof(T), value).ToString();

        public static async Task CreateIfNotExistAsync(this RoleManager<IdentityRole> roleManager, string name)
        {
            if (roleManager == null)
                throw new ArgumentNullException(nameof(roleManager));

            if (name == null)
                throw new ArgumentNullException("Name can't be empty or null");


            if (!await roleManager.RoleExistsAsync(name))
            {
                await roleManager.CreateAsync(new IdentityRole
                {
                    Name = name
                });
            }
        }

        public static async Task<ApplicationUser> FindByCompanyId(this UserManager<ApplicationUser> userManager, int companyId) => await userManager.Users.SingleOrDefaultAsync(u => u.CompanyId == companyId);

        public static bool IsDateValid(this DateTime date)
        {
            if (date == null || date == DateTime.MinValue || date == DateTime.MaxValue)
            {
                return false;
            }

            DateTime parsedDate;
            return DateTime.TryParse(date.ToString(), out parsedDate);
        }
    }
}
