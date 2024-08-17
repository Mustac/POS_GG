using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using POS_GG_APP.Models;
using POS_OS_GG.Helpers;
using POS_OS_GG.Models;

namespace POS_GG_APP.Data;

public static class MyExtensionMethods
{
    public static string GetName<T>(this T value) where T : Enum => Enum.GetName(typeof(T), value).ToString();

    public static async Task CreateIfNotExistAsync(this RoleManager<IdentityRole> roleManager, string name)
    {
        if (roleManager == null)
            throw new ArgumentNullException(nameof(roleManager));

        if (name == null)
            throw new ArgumentNullException("Name can't be empty or null");


        if (!await roleManager.RoleExistsAsync(name))
        {
            await roleManager.CreateAsync(new IdentityRole
            {
                Name = name
            });
        }
    }

    public static async Task<ApplicationUser> FindByCompanyId(this UserManager<ApplicationUser> userManager, int companyId) => await userManager.Users.SingleOrDefaultAsync(u => u.CompanyId == companyId);

    public static bool IsDateValid(this DateTime date)
    {
        if (date == null || date == DateTime.MinValue || date == DateTime.MaxValue)
        {
            return false;
        }

        DateTime parsedDate;
        return DateTime.TryParse(date.ToString(), out parsedDate);
    }

    public static OrderDTO ToDTO(this Order order)
    {
        var orderDTO = new OrderDTO
        {
            OrderId = order.Id,

            TimeOrdered = order.TimeOrdered.AddHours(2),
            TimeDelivered = order.TimeDelivered?.AddHours(2),
            OrderStatus = (OrderStatus)order.Status,
            OrderedProducts = order.OrderProducts.Select(op => new ProductInfo
            {
                Id = op.Product.Id,
                Name = op.Product.Name,
                CategoryId = op.Product.CategoryId,
                CategoryName = op.Product.Category.Name,
                CategoryIcon = op.Product.Category.Icon, // Assuming Category has an Icon property
                Measurement = (Measurement)op.Measurement,
                Quantity = op.Quantity
            }).ToList(),
            Message = order.Message,
            UserDeliveredId = order.UserDeliveredId,
             UserDeliveredName = order.UserDelivered?.UserName,
             UserOrderedName = order.UserOrdered?.UserName
        };

        if (order.TimeOrderTaken.HasValue)
        {
            orderDTO.TimeOrderTaken = order.TimeOrderTaken.Value.AddHours(2);
        }

        if (order.TimeDelivered.HasValue) {
            orderDTO.TimeDelivered = order.TimeDelivered.Value.AddHours(2);
        }

        return orderDTO;
    }

    public static int GetPassedMinutes(this DateTime startTime)
    {
        var time = Math.Abs((int)(DateTime.UtcNow.AddHours(2) - startTime).TotalMinutes);
        return time;
    }


}
