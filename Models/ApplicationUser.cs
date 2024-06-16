using Microsoft.AspNetCore.Identity;
using POS_OS_GG.Models;

namespace POS_GG_APP.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int CompanyId { get; set; }
        public List<Product> Products { get; set; }
        public List<Order> Orders { get; set; }
    }

}
