using POS_OS_GG.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_OS_GG.Models.ViewModels
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public DateTime TimeOrdered { get; set; }
        public DateTime TimeDelivered { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.None;
        public IEnumerable<ProductInfo> OrderedProducts { get; set; } = new List<ProductInfo>();
        public string Message { get; set; } = string.Empty;
        public string UserDeliveredId { get; set; } = string.Empty;
        public string UserDeliveredName { get; set; } = string.Empty;
        public string UserOrderedName { get; set; } = string.Empty;

    }
}
