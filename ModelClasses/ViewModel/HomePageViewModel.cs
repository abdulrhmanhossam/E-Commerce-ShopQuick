
namespace ModelClasses.ViewModel
{
    public class HomePageViewModel
    {
        public IEnumerable<Product> ProductList { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string searchName { get; set; }
    }
}
