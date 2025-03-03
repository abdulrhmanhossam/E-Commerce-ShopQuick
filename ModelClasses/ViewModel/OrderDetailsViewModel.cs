using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses.ViewModel
{
    public class OrderDetailsViewModel
    {
        public UserOrderHeader? OrderHeader { get; set; }
        public IEnumerable<OrderDetails> UserProductList { get; set; }
    }
}
