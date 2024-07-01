using Microsoft.CodeAnalysis.CSharp.Syntax;
using MudBlazor;
using POS_OS_GG.Data;
using POS_OS_GG.Helpers;
using POS_OS_GG.Models;
using POS_OS_GG.Models.ViewModels;

namespace POS_OS_GG.Services
{
    public class OrderService : BaseService
    {
        public OrderService(ApplicationDbContext appDbContext, GlobalManager globalManager, ISnackbar snackbar) : base(appDbContext, globalManager, snackbar)
        {
        }

        public async Task<RequestResponse> OrderProductsAsync(List<ProductInfo> orderProducts, string userId)
        {
            try
            {
                Order order = new Order()
                {
                    TimeOrdered = DateTime.UtcNow,
                    UserOrderedId = userId,
                    OrderProducts = new List<OrderProduct>(),
                };

                foreach(var orderProduct in orderProducts)
                {
                    order.OrderProducts.Add(new OrderProduct
                    {
                        OrderId = order.Id,
                        ProductId = orderProduct.Id,
                        Quantity = orderProduct.Quantity,
                        Measurement = orderProduct.Measurement
                    });
                }

                await _context.Orders.AddAsync(order);

                return await _context.SaveChangesAsync() > 0 ? 
                    _response.Success() : _response.Fail();

            }
            catch
            {
                return _response.ServerError();
            }
        }
    }
}
