using System.ComponentModel.DataAnnotations;

namespace E_Commerce.ViewModel {
    public class ConfirmOrderViewModel {
        public int CustomerId {get; set;}

        [Required(ErrorMessage = "Please enter fullname")]
        public string FullName {get; set;}

        [Required(ErrorMessage = "Please enter phone number")]
        public string Phone {get; set;}

        // Khi đăng ký tài khoản không đăng kí address nên cần xác nhận ở Confirm Order 
        [Required(ErrorMessage = "Please enter address")]
        public string Address {get; set;}

        public int PaymentId {get; set;}
    }
}