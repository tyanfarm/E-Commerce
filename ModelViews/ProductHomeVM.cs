using E_Commerce.Models;

namespace E_Commerce.ModelViews {
    // Phân loại sản phẩm theo category để xuất hiện ở Home Page
    public class ProductHomeVM {
        public List<Product> listProducts {get; set;}
        public Category category {get; set;}
    }
}