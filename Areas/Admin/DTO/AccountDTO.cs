using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Areas.Admin.DTO {
    public class AccountDTO {
        [Key]
        [MaxLength(100)]
        [Required(ErrorMessage="Please enter username")]
        public string Username {get; set;}

        [Display(Name="Password")]
        [Required(ErrorMessage="Please enter password")]
        [MinLength(5, ErrorMessage="Minimum 5 characters")]
        public string Password {get; set;}

    }
}