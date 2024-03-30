using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ModelClasses
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public UserOrderHeader? OrderHeader { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        // quantity of each unique item for the specific individual UserOreserHeader
        [Required]
        public int Count { get; set; }
        // total price of individual item from the cart
        [Required]
        public double Price { get; set; }
    }
}
