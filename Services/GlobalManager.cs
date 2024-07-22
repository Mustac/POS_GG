using Microsoft.AspNetCore.Identity;
using POS_OS_GG.Models.ViewModels;

namespace POS_OS_GG.Services
{
    public class GlobalManager
    {
        public HashSet<UserInfo>? Users { get; set; }
        public HashSet<ProductInfo>? Products { get; set; }

        public UserCallBack UserEvents { get; set; } = new UserCallBack();

        public ProductCallBack ProductEvents { get; private set; } = new ProductCallBack();
        public OrderCallBack OrderEvents { get; private set; }= new OrderCallBack();

        public class UserCallBack
        {
            public Action? OnUsersChange { get; set; }
        }

        public class ProductCallBack
        {
            public Action? OnProductsChange { get; set; }
        }

        public class OrderCallBack 
        {
            public Action<List<OrderDTO>>? OnOrderMade;
            public Func<Task> OnUserOrderCancelAsync;
        }
    }
}
