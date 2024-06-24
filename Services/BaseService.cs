using Azure;
using MudBlazor;
using POS_OS_GG.Data;
using POS_OS_GG.Helpers;

namespace POS_OS_GG.Services
{
    public abstract class BaseService
    {
        protected readonly ApplicationDbContext _context;
        protected readonly GlobalManager _globalManager;
        private readonly ISnackbar _snackbar;
        protected readonly ILogger _logger;
        protected readonly ResponseCreator _response;

        public BaseService(ApplicationDbContext appDbContext, GlobalManager globalManager, ISnackbar snackbar, ILogger logger)
        {
            _context = appDbContext;
            _globalManager = globalManager;
            _snackbar = snackbar;
            _logger = logger;
            _response = new ResponseCreator(snackbar);
        }
    }
}
