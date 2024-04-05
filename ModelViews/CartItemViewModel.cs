using E_Commerce.Models;

namespace E_Commerce.ModelViews {
    public class CartItemViewModel {
        public Product product {get; set;}

        public int quantity {get; set;}

        // .Value because "cannot convert type int -> double"
        public double totalMoney => quantity * product.Price.Value;

    }
}