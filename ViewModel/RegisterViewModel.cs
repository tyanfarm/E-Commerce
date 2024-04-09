using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.ViewModel 
{
    public class RegisterViewModel 
    {
        [Key]
        public int CustomerId {get; set;}

        [Display(Name="Full Name")]
        [Required(ErrorMessage = "Please enter Full name")]
        public string FullName {get; set;}

        [Display(Name="Email")]
        [Required(ErrorMessage = "Please enter Email")]
        [MaxLength(150)]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "ValidateEmail", controller: "UserAccount")]
        public string Email {get; set;}

        [Display(Name="Phone")]
        [Required(ErrorMessage = "Please enter Phone number")]
        [MaxLength(11)]
        [DataType(DataType.PhoneNumber)]
        // AJAX - (Asynchronous JavaScript and XML)
        // Request AJAX to action in a controller
        [Remote(action: "ValidatePhone", controller: "UserAccount")]
        public string Phone {get; set;}

        [Display(Name="Password")]
        [Required(ErrorMessage="Please enter password")]
        [MinLength(5, ErrorMessage="Minimum 5 characters")]
        public string Password {get; set;}
        
        [Display(Name="Confirm Password")]
        [Compare("Password", ErrorMessage = "Password isn't correct")]
        [MinLength(5, ErrorMessage="Minimum 5 characters")]
        public string ConfirmPassword {get; set;}
    }
}