using Microsoft.AspNetCore.Identity;
using POS_OS_GG.Data;
using POS_OS_GG.Models;
using POS_OS_GG.Models.ViewModels;

namespace POS_OS_GG.Services
{
    public class GlobalManager
    {
        public HashSet<UserInfo>? Users { get; set; }
        public HashSet<ProductInfo>? Products { get; set; }


        public UserCallBack UserEvents { get; set; } = new UserCallBack();

        public ProductCallBack ProductEvents { get; set; } = new ProductCallBack();

        public class UserCallBack
        {
            public Action? OnUsersChange { get; set; }
        }

        public class ProductCallBack
        {
            public Action? OnProductsChange { get; set; }
        }

        public async Task SeedData(IHost app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var database = services.GetRequiredService<ApplicationDbContext>();

                    var globalManager = services.GetRequiredService<GlobalManager>();

                    globalManager.Products = (await database.Products.Include(x => x.Category)
                        .Select(x => new ProductInfo { Id = x.Id, Name = x.Name, CategoryId = x.CategoryId, CategoryName = x.Category.Name, CategoryIcon = x.Category.Icon })
                        .ToListAsync()).ToHashSet();

                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while creating the admin user.");
                }
            }
        }

       
    }
}
