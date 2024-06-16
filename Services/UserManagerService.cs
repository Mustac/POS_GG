using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using MudBlazor;
using POS_GG_APP.Data;
using POS_OS_GG.Data;
using POS_OS_GG.Helpers;
using POS_OS_GG.Models.ViewModels;

namespace POS_OS_GG.Services
{
    public class UserManagerService
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly GlobalManager _globalManager;
        private readonly ResponseCreator response;

        public UserManagerService(ApplicationDbContext appDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, GlobalManager globalManager, ISnackbar snackbar)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _globalManager = globalManager;
            response = new ResponseCreator(snackbar);
        }

        /// <summary>
        /// Get all the Users and save it to Users object for catching
        /// </summary>
        /// <returns></returns>
        public async Task<RequestResponse<HashSet<UserInfo>>> GetUsersAsync()
        {
            try
            {
                if (_globalManager.Users is null)
                    _globalManager.Users = await UpdateGlobalUserList();

                return response.Success(_globalManager.Users, notification:false);
            }
            catch
            {
                return response.ServerError<HashSet<UserInfo>>();
            }
        }

        /// <summary>
        /// Deletes a user identified by the provided userId and triggers a user change event.
        /// </summary>
        /// <param name="userId">The ID of the user to be deleted.</param>
        /// <returns>A Task representing the asynchronous operation, containing a RequestResponse indicating the result of the deletion.</returns>
        public async Task<RequestResponse> DeleteAsync(string userId)
        {
            try
            {
                // Find the user by their ID
                var user = await _userManager.FindByIdAsync(userId);

                // Check if the user exists
                if (user == null)
                {
                    return response.Fail("User could not be found", notification: false);
                }

                // Attempt to delete the user
                var deleteResult = await _userManager.DeleteAsync(user);

                // Check if the deletion was successful
                if (!deleteResult.Succeeded)
                {
                    return response.Fail("User could not be deleted", notification: false);
                }

                // Update the global user list and trigger the user change event
                _globalManager.Users = await UpdateGlobalUserList();
                _globalManager.UserEvents.OnUsersChange?.Invoke();

                return response.Success("User has been deleted", notification: false);
            }
            catch
            {
                return response.ServerError();
            }
        }


        /// <summary>
        /// Create a new user and triggers a user change event.
        /// </summary>
        /// <param name="userRegistration"></param>
        /// <returns></returns>
        // Creates a new user and updates the user's role
        public async Task<RequestResponse> CreateAsync(UserRegistration userRegistration)
        {
            try
            {
                // Check if user already exists by CompanyId
                var existingUser = await _userManager.FindByCompanyId(userRegistration.CompanyId.Value);
                if (existingUser != null)
                {
                    return response.Fail("User already exists", notification: true);
                }

                // Create new user
                var newUser = new ApplicationUser
                {
                    UserName = userRegistration.Name,
                    CompanyId = userRegistration.CompanyId.Value,
                };

                var createResult = await _userManager.CreateAsync(newUser, userRegistration.Password);
                if (!createResult.Succeeded)
                {
                    return response.Fail(createResult.Errors.FirstOrDefault().Description, notification: true);
                }

                // Add user to the specified role
                var addToRoleResult = await _userManager.AddToRoleAsync(newUser, userRegistration.Role);
                if (!addToRoleResult.Succeeded)
                {
                    // If adding to role fails, delete the newly created user
                    await _userManager.DeleteAsync(newUser);
                    return response.Fail("Unable to add user to a role", notification: true);
                }

                // Update global user list and trigger user change event
                _globalManager.Users = await UpdateGlobalUserList();
                _globalManager.UserEvents.OnUsersChange?.Invoke();

                return response.Success("User creation was successful", notification: true);
            }
            catch
            {
                return response.ServerError();
            }
        }


        /// <summary>
        /// Updates the role of a user and triggers a user change event.
        /// </summary>
        /// <param name="user">The user information containing the updated role.</param>
        /// <returns>A Task representing the asynchronous operation, containing a RequestResponse indicating the result of the update.</returns>
        // Updates the role of a user and triggers a user change event
        public async Task<RequestResponse> UpdateAsync(UserEdit user)
        {
            try
            {
                // Find the user by their ID
                var userToUpdate = await _userManager.FindByIdAsync(user.Id);

                // Check if the user exists
                if (userToUpdate == null)
                {
                    return response.Fail("User could not be found", notification: false);
                }

                userToUpdate.UserName = user.Name;
                userToUpdate.CompanyId = user.CompanyId.Value;

                if (user.PasswordChange)
                {
                    var passwordRemoveResult = await _userManager.RemovePasswordAsync(userToUpdate);
                    var passwordSetResult = await _userManager.AddPasswordAsync(userToUpdate, user.Password);
                    if (!passwordRemoveResult.Succeeded || !passwordSetResult.Succeeded)
                    {
                        return response.Fail("Password could not be changed", notification: false);
                    }
                }

                var result = await _userManager.UpdateAsync(userToUpdate);

                if (!result.Succeeded)
                {
                    return response.Fail("Unable to update user", notification: true);
                }

                // Update the user's role
                var currentRole = await _userManager.GetRolesAsync(userToUpdate);
                if (currentRole != null && currentRole.FirstOrDefault() != user.Role)
                {
                    var updateRoleResult = await _userManager.RemoveFromRolesAsync(userToUpdate, currentRole);

                    if (!updateRoleResult.Succeeded)
                    {
                        return response.Fail("Unable to update user role", notification: true);
                    }

                    var addToRoleResult = await _userManager.AddToRoleAsync(userToUpdate, user.Role);

                    if (!addToRoleResult.Succeeded)
                    {
                        return response.Fail("Unable to update user role", notification: true);
                    }
                }

                // Update global user list and trigger user change event
                _globalManager.Users = await UpdateGlobalUserList();
                _globalManager.UserEvents.OnUsersChange?.Invoke();

                return response.Success("User role has been updated", notification: true);
            }
            catch
            {
                return response.ServerError();
            }
        }


        /// <summary>
        /// Retrives all the Users and parse it to HashSet<UserInfo>
        /// </summary>
        /// <returns></returns>
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
