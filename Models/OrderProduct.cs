using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_OS_GG.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }
        public int Quantity { get; set; }


        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }


        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}