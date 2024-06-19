using Microsoft.AspNetCore.Identity;
using MudBlazor;
using POS_OS_GG.Data;
using POS_OS_GG.Helpers;
using POS_OS_GG.Models;
using POS_OS_GG.Models.ViewModels;

namespace POS_OS_GG.Services
{
    public class ProductService : BaseService
    {

        public ProductService(ApplicationDbContext applicationDbContext, GlobalManager globalManager, ISnackbar snackbar) : base(applicationDbContext, globalManager, snackbar)
        {
        }


        public async Task<RequestResponse<HashSet<ProductInfo>>> GetProductsAsync(string searchText = "")
        {
            try
            {
                if (_globalManager.Products == null)
                {
                    _globalManager.Products = await UpdateProductsAsync();
                }

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    return _response.Success(_globalManager.Products);
                }

                var searchLower = searchText.ToLower();

                var filteredProducts = _globalManager.Products
                    .Where(x => x.Name.ToLower().Contains(searchLower) ||
                                x.Id.ToString().Contains(searchText) ||
                                x.CategoryName.ToLower().Contains(searchLower))
                    .OrderByDescending(x=>x.Name)
                    .ToHashSet();

                return _response.Success(filteredProducts, notification:false);
            }
            catch (Exception ex)
            {
                return _response.ServerError<HashSet<ProductInfo>>(message: ex.Message);
            }
        }


        public async Task<RequestResponse<ProductInfo>> RegisterProductAsync(string productName)
        {
            try
            {
                var product = _globalManager.Products.SingleOrDefault(x=>x.Name == productName);

                if (product != null)
                {
                    return _response.Fail<ProductInfo>(null, notification: false);
                }

                Product productInfo = new Product
                {
                    Name = productName,
                    CategoryId = 1
                };

                await _context.Products.AddAsync(productInfo);
                var saveSuccess = await _context.SaveChangesAsync() > 0;

                if (saveSuccess)
                {
                    _globalManager.Products = await UpdateProductsAsync();
                    _globalManager.ProductEvents.OnProductsChange?.Invoke();
                }
                
                product = _globalManager.Products.SingleOrDefault(x=>x.Name == productName);

                return _response.Success(product, notification: false);

            }
            catch
            {
                return _response.ServerError<ProductInfo>();
            }
        }


        private async Task<HashSet<ProductInfo>> UpdateProductsAsync()
        {
           return (await _context.Products.Include(x => x.Category)
                .Select(x => new ProductInfo { Id = x.Id, Name = x.Name, CategoryId = x.CategoryId, CategoryName = x.Category.Name, CategoryIcon = x.Category.Icon })
                .ToListAsync()).ToHashSet();
        }}


    }
}
