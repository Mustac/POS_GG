using System.ComponentModel.DataAnnotations.Schema;

namespace POS_OS_GG.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime TimeOrdered { get; set; }
        public DateTime TimeDelivered { get; set; }
        public string UserOrderedId { get; set; }
        public string UserDeliveredId { get; set; }

        [ForeignKey("UserOrdered")]
        public int UserOrdered_Id { get; set; }
        public ApplicationUser UserOrdered { get; set; }

        [ForeignKey("UserDelivered")]
        public int UserDelivered_Id { get; set; }
        public ApplicationUser UserDelivered { get; set; }
        public List<OrderProduct> OrderProducts { get; set; }
    }
}
