using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ModelClasses.ViewModel
{
    public class ProductViewModel
    {
        public Product? Products { get; set; }
        public IEnumerable<SelectListItem>? CategoriesList { get; set; }
        public IEnumerable<IFormFile>? Images { get; set; }
        public Inventory? Inventories { get; set; }
        public ProductImages? ProductImages { get; set; }

    }
}
