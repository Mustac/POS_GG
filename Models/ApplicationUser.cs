using Microsoft.AspNetCore.Identity;
using POS_OS_GG.Models;

namespace POS_GG_APP.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int CompanyId { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Order> OrdersMade { get; set; }
        public ICollection<Order> OrdersDelivered { get; set; }

    }

}
