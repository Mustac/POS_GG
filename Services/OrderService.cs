using Microsoft.CodeAnalysis.CSharp.Syntax;
using MudBlazor;
using POS_GG_APP.Data;
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

        public async Task<RequestResponse> OrderProductsAsync(List<ProductInfo> orderProducts, string message, string userId)
        {
            try
            {
                Order order = new Order()
                {
                    TimeOrdered = DateTime.UtcNow,
                    UserOrderedId = userId,
                    OrderProducts = new List<OrderProduct>(),
                };

                foreach (var orderProduct in orderProducts)
                {
                    order.OrderProducts.Add(new OrderProduct
                    {
                        OrderId = order.Id,
                        ProductId = orderProduct.Id,
                        Quantity = orderProduct.Quantity,
                        Measurement = (int)orderProduct.Measurement,
                        Order = new Order()
                        {
                            Message = message,
                            TimeOrdered = DateTime.UtcNow,
                            UserOrderedId = userId
                        }
                    });
                }

                await _context.Orders.AddAsync(order);

                return await _context.SaveChangesAsync() > 0 ?
                    _response.Success(message: "Your order has been made") :
                    _response.Fail(message: "Order could not be made, please try again");

            }
            catch
            {
                return _response.ServerError();
            }
        }

        public async Task<RequestResponse<IEnumerable<OrderDTO>>> GetOrdersAsync(string userId)
        {

            try
            {

                var orders = await _context.Orders.Where(x => x.UserOrderedId == userId).Include(x => x.OrderProducts).ThenInclude(x => x.Product).Include(x => x.UserDelivered).DefaultIfEmpty().ToListAsync();

                if (orders is null)
                    return _response.NoContent<IEnumerable<OrderDTO>>(notification: false);

                List<OrderDTO> ordersDTO = new();


                foreach (var order in orders)
                {
                    OrderDTO tempOrder = new OrderDTO
                    {
                        Message = order.Message,
                        OrderId = order.Id,
                        OrderStatus = order.TimeOrdered.IsDateValid() ? OrderStatus.Ordered : OrderStatus.OrderDelivered,
                        TimeDelivered = order.TimeDelivered.IsDateValid() ? order.TimeDelivered : default,
                        TimeOrdered = order.TimeOrdered,
                        UserDeliveredId = order.UserDeliveredId ?? "",
                        UserDeliveredName = order.UserDelivered?.UserName ?? "",
                        OrderedProducts = order.OrderProducts.Select(x => new ProductInfo
                        {
                            Id = x.ProductId,
                            Measurement = (Measurement)x.Measurement,
                            Name = x.Product.Name,
                            Quantity = x.Quantity
                        })
                    };

                    ordersDTO.Add(tempOrder);
                }

                return _response.Success<IEnumerable<OrderDTO>>(ordersDTO, notification: false);


            }
            catch (Exception ex)
            {
                return _response.ServerError<IEnumerable<OrderDTO>>(message: ex.Message);
            }
        }

        public async Task<RequestResponse> RemoveOrderAsync(int id)
        {
            try
            
            {
                var order = await _context.Orders.FindAsync(id);

                if (order == null)
                    return _response.NoContent(message:"Could not find the order, please refresh the page", notification:true);

                _context.Remove(order);

                return await _context.SaveChangesAsync() > 0 ?
                    _response.Success(message:"Order has been deleted") :
                    _response.Fail();
            }
            catch (Exception ex)
            {
                return _response.ServerError(message: ex.Message);
            }

        }

    }


}
