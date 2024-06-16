using System.ComponentModel.DataAnnotations.Schema;

namespace POS_OS_GG.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string UserRegistratedId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [ForeignKey("ApplicationUser")]
        public string ApplicationUser_Id { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; }
    }
}
