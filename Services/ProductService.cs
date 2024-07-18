using Microsoft.AspNetCore.Identity;
using MudBlazor;
using POS_OS_GG.Data;
using POS_OS_GG.Helpers;
using POS_OS_GG.Models;
using POS_OS_GG.Models.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;


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
                return _response.Success(_globalManager.Products.Take(10).ToHashSet<ProductInfo>(), notification: false);
            }

            var filteredProducts = _globalManager.Products
                .Where(x => x.Name.ToUpper().Contains(searchText.ToUpper()))
                .OrderBy(x=>x.Name)
                .Take(10)
                .ToHashSet();

            return _response.Success(filteredProducts, notification:false);
        }
        catch (Exception ex)
        {
            return _response.ServerError<HashSet<ProductInfo>>(message: ex.Message);
        }
    }


    public async Task<RequestResponse> RegisterProductAsync(string productName, string userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);

            if(user is null)
                  return _response.Fail(message:"User does not exist, please relog" , notification: true);

            var product = await _context.Products.FirstOrDefaultAsync(x=>x.Name.ToUpper() == productName.ToUpper());

            if(product is not null)
                return _response.Fail(message: "Product already exists", notification: true);

            Product productInfo = new Product
            {
                Name = productName.ToUpper(),
                CategoryId = 1,
                UserRegistratedId = userId,
            };
            
            await _context.Products.AddAsync(productInfo);
            var saveSuccess = await _context.SaveChangesAsync() > 0;

            if (saveSuccess)
            {
                _globalManager.Products = await UpdateProductsAsync();
                _globalManager.ProductEvents.OnProductsChange?.Invoke();
            }
            
            return _response.Success(notification: true);

        }
        catch
        {
            return _response.ServerError();
        }
    }
    public async Task<RequestResponse<ProductInfo>> GetProductAsync(string searchText = "")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return _response.Fail<ProductInfo>(null, notification: false);
            }

            var product = await _context.Products.Include(x => x.Category).Select(x => new ProductInfo
            {
                Id = x.Id,
                Name = x.Name,
            }).FirstOrDefaultAsync(p => p.Name.ToUpper() == searchText.ToUpper());

            if (product == null)
            {
                return _response.Fail<ProductInfo>(null, message: "Product not found.");
            }

            return _response.Success(product, notification: false);
        }
        catch (Exception ex)
        {
            return _response.ServerError<ProductInfo>(message: ex.Message);
        }
    }


    private async Task<HashSet<ProductInfo>> UpdateProductsAsync()
    {
       return (await _context.Products.Include(x => x.Category)
            .Select(x => new ProductInfo { Id = x.Id, Name = x.Name, CategoryId = x.CategoryId, CategoryName = x.Category.Name, CategoryIcon = x.Category.Icon })
            .ToListAsync()).ToHashSet();
    }}



