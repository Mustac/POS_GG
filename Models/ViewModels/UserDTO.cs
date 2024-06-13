using System.ComponentModel.DataAnnotations;

namespace POS_OS_GG.Models.ViewModels
{
    public class UserInfo
    {
        public string Id { get; set; } = string.Empty;
        [Required]
        [Range(minimum:100, maximum:99999, ErrorMessage ="Id has to be from 3 to 5 digits")]
        public int? CompanyId { get; set; }
        [Required]
        [StringLength(maximumLength:30, MinimumLength =3, ErrorMessage ="Name has to be between 3 and 30 characters")]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
    }

    public class UserRegistration
    {
        [Required]
        [Range(minimum: 100, maximum: 99999, ErrorMessage = "Id has to be from 3 to 5 digits")]
        public int? CompanyId { get; set; }
        [Required]
        [StringLength(maximumLength: 30, MinimumLength = 3, ErrorMessage = "Name has to be between 3 and 30 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(maximumLength: 30, MinimumLength = 4, ErrorMessage = "Password has to be between 4 and 10 characters")]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
    }

    public class UserEdit
    {
        public string Id { get; set; } = string.Empty;
        [Required]
        [Range(minimum: 100, maximum: 99999, ErrorMessage = "Id has to be from 3 to 5 digits")]
        public int? CompanyId { get; set; }
        [Required]
        [StringLength(maximumLength: 30, MinimumLength = 3, ErrorMessage = "Name has to be between 3 and 30 characters")]
        public string Name { get; set; } = string.Empty;
        public bool PasswordChange { get; set; }

        [Required(AllowEmptyStrings=true)]
        [StringLength(maximumLength: 30, MinimumLength = 4, ErrorMessage = "Password has to be between 4 and 10 characters")]
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

    }
}
