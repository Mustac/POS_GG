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
                    Message = message,
                    Status = (int)OrderStatus.OrderStarted
                };

                foreach (var orderProduct in orderProducts)
                {
                    order.OrderProducts.Add(new OrderProduct
                    {
                        OrderId = order.Id,
                        ProductId = orderProduct.Id,
                        Quantity = orderProduct.Quantity,
                        Measurement = (int)orderProduct.Measurement,
                    });
                }

                var orderResponse = await _context.Orders.AddAsync(order);

                var saveSuccess = await _context.SaveChangesAsync() > 0;

                if (saveSuccess)
                {
                    var orders = await GetOrdersAsync();
                   
                    if(orders is not null && orders.IsSuccess)
                        _globalManager.OrderEvents.OnOrderMade?.Invoke(orders.Data);
                }

                return saveSuccess ?
                    _response.Success(message: "Your order has been made") :
                    _response.Fail(message: "Order could not be made, please try again");

            }
            catch
            {
                return _response.ServerError();
            }
        }

        public async Task<RequestResponse<List<OrderDTO>>> GetOrdersAsync(string userId="")
        {

            try
            {
                List<Order> orders;

                if(userId != "")
                {
                    orders = await _context.Orders.Where(x => x.UserOrderedId == userId && x.TimeOrdered.Date == DateTime.UtcNow.Date).Include(x => x.OrderProducts).ThenInclude(x => x.Product).ThenInclude(x=>x.Category).Include(x => x.UserDelivered).DefaultIfEmpty().ToListAsync();
                }
                else
                {
                    orders = await _context.Orders.Where(x => x.TimeOrdered.Date == DateTime.UtcNow.Date).Include(x => x.OrderProducts).ThenInclude(x => x.Product).ThenInclude(x=>x.Category).Include(x => x.UserDelivered).Include(x=>x.UserOrdered).DefaultIfEmpty().ToListAsync();
                }

                if (orders is null || orders.Count == 0)
                        return _response.NoContent<List<OrderDTO>>(notification: false);

                List<OrderDTO> ordersDTO = new();

                foreach (var order in orders)
                {
                    if (order is null)
                        continue;

                    ordersDTO.Add(order.ToDTO());
                }

                return _response.Success<List<OrderDTO>>(ordersDTO, notification: false);


            }
            catch (Exception ex)
            {
                return _response.ServerError<List<OrderDTO>>(message: ex.Message);
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

                var removeResponse = await _context.SaveChangesAsync() > 0;

                if (removeResponse)
                {
                    await _globalManager.OrderEvents?.OnUserOrderCancelAsync();
                }

                return removeResponse ?
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
