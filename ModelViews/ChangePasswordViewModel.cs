using System.ComponentModel.DataAnnotations;

namespace E_Commerce.ModelViews {
    // Phân loại sản phẩm theo category để xuất hiện ở Home Page
    public class ChangePasswordViewModel {
        [Key]
        public int CustomerId {get; set;}

        [Display(Name="Password Now")]
        public string PasswordNow {get; set;}

        [Display(Name="New Password")]
        [MinLength(5, ErrorMessage="Minimum 5 characters")]
        [Required(ErrorMessage = "Please enter password")]
        public string Password {get; set;}

        [Display(Name="Confirm Password")]
        [Compare("Password", ErrorMessage = "Password isn't correct")]
        [MinLength(5, ErrorMessage="Minimum 5 characters")]
        public string ConfirmPassword {get; set;}
    }
}