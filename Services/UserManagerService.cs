using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using POS_GG_APP.Data;
using POS_OS_GG.Data;
using POS_OS_GG.Models.ViewModels;

namespace POS_OS_GG.Services
{
    public class UserManagerService
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly GlobalManager _globalManager;

        public UserManagerService(ApplicationDbContext appDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, GlobalManager globalManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _globalManager = globalManager;
        }

        // get all users
       public async Task<HashSet<UserInfo>> GetUsersAsync()
        {
            if(_globalManager.Users is null)
               _globalManager.Users = await UpdateGlobalUserList();

            return _globalManager.Users;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return false;

            _globalManager.Users = await UpdateGlobalUserList();
            _globalManager.UserEvents.OnUsersChange?.Invoke();

            return result.Succeeded;
        }

        public async Task<bool> CreateUserAsync(UserRegistration userRegistration)
        {
            //create user and to the database
            var newUser = new ApplicationUser
            {
                UserName = userRegistration.Name,
                CompanyId = userRegistration.CompanyId.Value,
            };

            var result = await _userManager.CreateAsync(newUser, userRegistration.Password);

            if (!result.Succeeded)
                return false;
            
            var resultSuccess = await _userManager.AddToRoleAsync(await _userManager.GetUserByCompanyId(userRegistration.CompanyId.Value), userRegistration.Role);

            if (!resultSuccess.Succeeded)
                return false;

            _globalManager.Users = await UpdateGlobalUserList();
            _globalManager.UserEvents.OnUsersChange?.Invoke();

            return true;
        }

        private async Task<HashSet<UserInfo>> UpdateGlobalUserList()
        {
            var users = (await _appDbContext.UserRoles
                .Join(_appDbContext.ApplicationUsers, x => x.UserId, x => x.Id, (role, user) => new { role, user })
                .Join(_appDbContext.Roles, x => x.role.RoleId, x => x.Id, (userRole, role) => new { userRole, role })
                .Select(ur =>
                    new UserInfo
                    {
                        Id = ur.userRole.user.Id,
                        CompanyId = ur.userRole.user.CompanyId,
                        Name = ur.userRole.user.UserName,
                        Role = ur.role.Name
                    }).ToListAsync()).ToHashSet();

            return users;
        }
    }
}
