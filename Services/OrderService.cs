using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using POS_GG_APP.Data;
using POS_OS_GG.Data;
using POS_OS_GG.Helpers;
using POS_OS_GG.Models;
using POS_OS_GG.Models.ViewModels;

namespace POS_OS_GG.Services;

public class OrderService : BaseService
{
    private readonly UserManagerService _userManagerService;

    public OrderService(DbContextOptions<ApplicationDbContext> options, GlobalManager globalManager, ISnackbar snackbar, UserManagerService userManagerService) : base(options, globalManager, snackbar)
    {
        _userManagerService = userManagerService;
    }


    public async Task<RequestResponse> OrderProductsAsync(List<ProductInfo> orderProducts, string message, string userId)
     => await UseDbContextInstanceAsync(async context =>
     {
         Order order = new Order()
         {
             TimeOrdered = DateTime.UtcNow,
             UserOrderedId = userId,
             OrderProducts = new List<OrderProduct>(),
             Message = message,
             Status = (int)OrderStatus.Started
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

         var orderResponse = await context.Orders.AddAsync(order);

         var saveSuccess = await context.SaveChangesAsync() > 0;

         if (saveSuccess)
         {
             var orders = await GetOrdersAsync(orderStatus:OrderStatus.Started);

             if (orders is not null && orders.IsSuccess && _globalManager.OrderEvents.OnOrderMade is not null)
                 _globalManager.OrderEvents.OnOrderMade.Invoke(orders.Data);
         }

         return saveSuccess ?
             _response.Success(message: "Your order has been made") :
             _response.Fail(message: "Order could not be made, please try again");

     });

    public async Task<RequestResponse<List<OrderDTO>>> GetOrdersAsync(string userId = "", OrderStatus orderStatus = OrderStatus.None)
     => await UseDbContextInstanceAsync(async context =>
     {
         List<Order> orders;

         if (userId != "")
         {
             if (orderStatus == OrderStatus.None)
             {
                 orders = await context.Orders.Where(x => x.UserOrderedId == userId && x.TimeOrdered.Date == DateTime.UtcNow.AddHours(2).Date).Include(x => x.OrderProducts).ThenInclude(x => x.Product).ThenInclude(x => x.Category).Include(x => x.UserDelivered).DefaultIfEmpty().ToListAsync();
             }
             else
             {
                 orders = await context.Orders.Where(x => x.UserOrderedId == userId && x.TimeOrdered.Date == DateTime.UtcNow.AddHours(2).Date && x.Status == (int)orderStatus).Include(x => x.OrderProducts).ThenInclude(x => x.Product).ThenInclude(x => x.Category).Include(x => x.UserDelivered).DefaultIfEmpty().ToListAsync();
             }
         }
         else
         {
             if (orderStatus == OrderStatus.None)
             {
                 orders = await context.Orders.Where(x => x.TimeOrdered.Date == DateTime.UtcNow.AddHours(2).Date).Include(x => x.OrderProducts).ThenInclude(x => x.Product).ThenInclude(x => x.Category).Include(x => x.UserDelivered).Include(x => x.UserOrdered).DefaultIfEmpty().ToListAsync();
             }
             else
             {
                 orders = await context.Orders.Where(x => x.TimeOrdered.Date == DateTime.UtcNow.AddHours(2).Date && x.Status == (int)orderStatus).Include(x => x.OrderProducts).ThenInclude(x => x.Product).ThenInclude(x => x.Category).Include(x => x.UserDelivered).Include(x => x.UserOrdered).DefaultIfEmpty().ToListAsync();
             }
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
     });

    public async Task<RequestResponse<OrderDTO>> GetOrderAsync(int id)
        => await UseDbContextInstanceAsync<OrderDTO>(async context =>
        {
            var order = await context.Orders.Include(x => x.OrderProducts).ThenInclude(x => x.Product).ThenInclude(x => x.Category).Include(x => x.UserDelivered).DefaultIfEmpty().FirstOrDefaultAsync(x => x.Id == id);

            if(order is null)
                return _response.NoContent<OrderDTO>(notification: false);

            var orderDTO = order.ToDTO();

            return _response.Success<OrderDTO>(data:orderDTO);
        });

    public async Task<RequestResponse> RemoveOrderAsync(int id)
            => await UseDbContextInstanceAsync(async context =>
            {
                var order = await context.Orders.FindAsync(id);

                if (order == null)
                    return _response.NoContent(message: "Could not find the order, please refresh the page", notification: true);

                context.Remove(order);

                var removeResponse = await context.SaveChangesAsync() > 0;

                if (removeResponse)
                {
                    if (_globalManager.OrderEvents.OnUserOrderCancel is not null)
                        await _globalManager.OrderEvents.OnUserOrderCancel.Invoke();
                }

                return removeResponse ?
                    _response.Success(message: "Order has been deleted") :
                    _response.Fail();
            });


    public async Task<RequestResponse> ChangeOrderStatusAsync(int orderId, OrderStatus orderStatus, string userId = "")
    => await UseDbContextInstanceAsync(async context =>
    {
        var order = await context.Orders
            .Include(x => x.OrderProducts).ThenInclude(x => x.Product).ThenInclude(x => x.Category)
            .Include(x => x.UserDelivered)
            .Include(x => x.UserOrdered)
            .FirstOrDefaultAsync(x => x.Id == orderId);

        if (order is null)
        {
            return _response.NoContent("This order could not be found", true);
        }

        order.Status = (int)orderStatus;

        if (orderStatus == OrderStatus.Taken && !string.IsNullOrEmpty(userId))
        {
            order.UserDeliveredId = userId;
        }

        if(orderStatus == OrderStatus.Started)
        {
            order.UserDeliveredId = null;
        }

        if(orderStatus == OrderStatus.Delivered)
        {
            order.TimeDelivered = DateTime.UtcNow;
        }

        var saveStatus = await context.SaveChangesAsync() > 0;

        if (saveStatus)
        {

            order = await context.Orders
            .Include(x => x.OrderProducts).ThenInclude(x => x.Product).ThenInclude(x => x.Category)
            .Include(x => x.UserDelivered)
            .Include(x => x.UserOrdered)
            .FirstOrDefaultAsync(x => x.Id == orderId);

            var orderDTO = order.ToDTO();

            _globalManager.OrderEvents?.OnOrderStatusChange?.Invoke(orderDTO);
        }

        return saveStatus ? _response.Success() : _response.Fail();
    });

}

