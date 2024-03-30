
namespace ModelClasses
{
    public class ProductImages
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string ImgUrl { get; set; }
        public string ProductName { get; set; }
    }
}