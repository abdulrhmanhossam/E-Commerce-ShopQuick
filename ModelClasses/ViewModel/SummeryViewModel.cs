
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ModelClasses.ViewModel
{
    public class SummeryViewModel
    {
        public IEnumerable<UserCart>? UserCartList { get; set; }
        public UserOrderHeader? OrderSummery { get; set; }
        public string? CartUserId { get; set; }
        public IEnumerable<SelectListItem>? PaymentOptions { get; set; }
        public double? PaymentPaidByCard { get; set; } = 0.0;
    }
}
