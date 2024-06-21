using Microsoft.AspNetCore.Identity;
using MudBlazor;
using POS_OS_GG.Data;
using POS_OS_GG.Helpers;
using POS_OS_GG.Models;
using POS_OS_GG.Models.ViewModels;

namespace POS_OS_GG.Services;

public class ProductService : BaseService
{

    public ProductService(ApplicationDbContext applicationDbContext, GlobalManager globalManager, ISnackbar snackbar) : base(applicationDbContext, globalManager, snackbar)
    {
    }


    public async Task<RequestResponse<HashSet<ProductInfo>>> GetProductsAsync(string searchText = "")
    {
        try
        {

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return _response.Success(_globalManager.Products, notification:false);
            }

            var searchLower = searchText.ToUpper();

            var filteredProducts = _globalManager.Products
                .Where(x => x.Name.Contains(searchLower) ||
                            x.CategoryName.Contains(searchLower))
                .OrderByDescending(x=>x.Name)
                .ToHashSet();

            return _response.Success(filteredProducts, notification:false);
        }
        catch (Exception ex)
        {
            return _response.ServerError<HashSet<ProductInfo>>(message: ex.Message);
        }
    }

    public async Task<RequestResponse<ProductInfo>> GetProductAsync(string searchText = "")
    {
        try
        {

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return _response.Fail<ProductInfo>(data:null, notification: false);
            }

            var searchLower = searchText.ToUpper();

            var product = _globalManager.Products
                .FirstOrDefault(x => x.Name.Contains(searchLower) ||
                            x.Id.ToString().Contains(searchText) ||
                            x.CategoryName.Contains(searchLower));

            return product is null ? _response.Fail<ProductInfo>(data:null, notification: false) : _response.Success(product, message:$"{product.Name} has been registrated", notification: false);
        }
        catch (Exception ex)
        {
            return _response.ServerError<ProductInfo>(message: ex.Message);
        }
    }


    public async Task<RequestResponse<ProductInfo>> RegisterProductAsync(string productName, string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(userId))
            {
                return _response.Fail<ProductInfo>(null, notification: true);
            }


            Product product = new Product
            {
                Name = productName.ToUpper(),
                CategoryId = 1,
                UserRegistratedId = userId
            };

            var productAddResponse = await _context.Products.AddAsync(product);
            var saveSuccess = await _context.SaveChangesAsync() > 0;

            if (saveSuccess)
            {
                _globalManager.Products = await UpdateProductsAsync();
                _globalManager.ProductEvents.OnProductsChange?.Invoke();
            }

            var productInfo = _globalManager.Products.FirstOrDefault(x => x.Id == productAddResponse.Entity.Id);
            
            return _response.Success(productInfo, notification: true);

        }
        catch (Exception ex)
        {
            return _response.ServerError<ProductInfo>(ex.Message.FirstOrDefault().ToString());
        }
    }


    private async Task<HashSet<ProductInfo>> UpdateProductsAsync()
    {
       return (await _context.Products.Include(x => x.Category)
            .Select(x => new ProductInfo { Id = x.Id, Name = x.Name, CategoryId = x.CategoryId, CategoryName = x.Category.Name, CategoryIcon = x.Category.Icon })
            .ToListAsync()).ToHashSet();
    }}




