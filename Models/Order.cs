using System.ComponentModel.DataAnnotations.Schema;

namespace POS_OS_GG.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime TimeOrdered { get; set; }
        public DateTime TimeDelivered { get; set; }


        [ForeignKey("UserOrdered")]
        public string UserOrderedId { get; set; }
        public ApplicationUser UserOrdered { get; set; }


        [ForeignKey("UserDelivered")]
        public string UserDeliveredId { get; set; }
        public ApplicationUser UserDelivered { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
