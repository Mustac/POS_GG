using Microsoft.AspNetCore.Identity;

namespace POS_GG_APP.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int CompanyId { get; set; }
    }

}
