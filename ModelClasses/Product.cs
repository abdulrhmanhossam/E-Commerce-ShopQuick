using System.ComponentModel.DataAnnotations;

namespace ModelClasses
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double Price  { get; set; }
        [Required]
        [MaxLength(2000, ErrorMessage = "Length can not exist more than 30 characters")]
        public string Description { get; set; }
        public ICollection<ProductImages>? ImgUrls { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? HomeImgUrl { get; set; }
    }
}