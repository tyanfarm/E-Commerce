using System.ComponentModel.DataAnnotations;

namespace E_Commerce.ModelViews 
{
    public class LoginViewModel 
    {
        [Key]
        [MaxLength(100)]
        [Required(ErrorMessage="Please enter username")]
        [DataType(DataType.EmailAddress)]
        [Display(Name="Email")]
        public string UserName {get; set;}

        [Display(Name="Password")]
        [Required(ErrorMessage="Please enter password")]
        [MinLength(5, ErrorMessage="Minimum 5 characters")]
        public string Password {get; set;}
    }
}