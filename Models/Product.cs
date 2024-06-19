using System.ComponentModel.DataAnnotations.Schema;

namespace POS_OS_GG.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;


        [ForeignKey("ApplicationUser")]
        public string UserRegistratedId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; }


        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
