using POS_OS_GG.Helpers;
using System.ComponentModel.DataAnnotations;

namespace POS_OS_GG.Models.ViewModels
{
    public class UserInfo
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [Range(minimum: 100, maximum: 99999, ErrorMessage = "Id has to be from 3 to 5 digits")]
        public int? CompanyId { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Name contains invalid characters.")]
        [StringLength(maximumLength: 30, MinimumLength = 3, ErrorMessage = "Name has to be between 3 and 30 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }

    public class UserRegistration : UserInfo
    {
        [Required]
        [StringLength(maximumLength: 30, MinimumLength = 4, ErrorMessage = "Password has to be between 4 and 10 characters")]
        public string Password { get; set; } = string.Empty;
    }

    public class UserEdit : UserInfo
    {
        public bool PasswordChange { get; set; }

        [RequiredIf(nameof(PasswordChange), 4, 10, ErrorMessage = "Password is required when changing password and must be between 4 and 10 characters.")]
        public string? Password { get; set; }
    }
}
