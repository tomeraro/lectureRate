using System.ComponentModel.DataAnnotations;

namespace lecturate.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "שם משתמש")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "סיסמא נוכחית")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "הסיסמא {0} צריכה להכיל לפחות {2} תווים", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "סיסמא חדשה")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "אישור סיסמא חדשה")]
        [Compare("NewPassword", ErrorMessage = "הסיסמאות החדשות אינן זהות, יש לנסות בשנית")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "שם משתמש")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "סיסמא")]
        public string Password { get; set; }

        [Display(Name = "זכור אותי?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "שם משתמש")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "הסיסמא {0} צריכה להכיל לפחות {2} תווים", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "סיסמא")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "אישור סיסמא")]
        [Compare("Password", ErrorMessage = "הסיסמאות אינן זהות, יש לנסות בשנית")]
        public string ConfirmPassword { get; set; }
    }
}
