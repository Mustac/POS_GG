using Azure;
using MudBlazor;
using POS_OS_GG.Data;
using POS_OS_GG.Helpers;

namespace POS_OS_GG.Services;

public abstract class BaseService
{
    protected readonly DbContextOptions<ApplicationDbContext> _options;
    protected readonly GlobalManager _globalManager;
    private readonly ISnackbar _snackbar;
    protected readonly ResponseCreator _response;

    public BaseService(DbContextOptions<ApplicationDbContext> options, GlobalManager globalManager, ISnackbar snackbar)
    {
        _options = options;
        _globalManager = globalManager;
        _snackbar = snackbar;
        _response = new ResponseCreator(snackbar);
    }

    protected async Task<RequestResponse> UseDbContextInstanceAsync(Func<ApplicationDbContext, Task<RequestResponse>> func)
    {
        try
        {
            ApplicationDbContext _context = new ApplicationDbContext(_options);
            var response = await func(_context);
            return response;
        }
        catch (Exception ex) 
        {
           return _response.ServerError(ex.Message.ToString());
        }
    }

    protected async Task<RequestResponse<T>> UseDbContextInstanceAsync<T>(Func<ApplicationDbContext, Task<RequestResponse<T>>> func)
    {
        try
        {
            ApplicationDbContext _context = new ApplicationDbContext(_options);
            var response = await func(_context);
            return response;
        }
        catch (Exception ex) 
        {
            return _response.ServerError<T>(ex.ToString());
        }
    }

}
