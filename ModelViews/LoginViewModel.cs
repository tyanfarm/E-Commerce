using System.ComponentModel.DataAnnotations;

namespace E_Commerce.ModelViews 
{
    public class LoginViewModel 
    {
        [MaxLength(100)]
        [Required(ErrorMessage="Please enter username")]
        [Display(Name="Email")]
        public string UserName {get; set;}

        [Display(Name="Password")]
        [Required(ErrorMessage="Please enter password")]
        [MinLength(5, ErrorMessage="Minimum 5 characters")]
        public string Password {get; set;}
    }
}