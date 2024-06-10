using Microsoft.AspNetCore.Identity;
using POS_OS_GG.Models.ViewModels;

namespace POS_OS_GG.Services
{
    public class GlobalManager
    {
        public HashSet<UserInfo>? Users { get; set; }

        public UserCallBacks UserEvents { get; set; } = new UserCallBacks();
        public class UserCallBacks
        {
            public Action? OnUsersChange { get; set; }
        }
    }
}
