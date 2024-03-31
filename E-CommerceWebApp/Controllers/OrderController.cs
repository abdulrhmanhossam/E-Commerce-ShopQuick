using DataBaseAccess;
using E_CommerceWebApp.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelClasses;
using ModelClasses.ViewModel;
using Stripe;
using Stripe.Checkout;

namespace E_CommerceWebApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        [BindProperty]
        public OrderDetailsViewModel OrderDetailsViewModel { get; set; }

        public OrderController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult OrderDetailPreview()
        {
            var claim = _signInManager.IsSignedIn(User);
            if (claim)
            {
                var userId = _userManager.GetUserId(User);
                var currentUser = _dbContext.ApplicationUsers.FirstOrDefault(x => x.Id == userId);
                SummeryViewModel summeryViewModel = new SummeryViewModel()
                {
                    UserCartList = _dbContext.UserCarts
                        .Include(x => x.Product)
                        .Where(x => x.UserId.Contains(userId!)).ToList(),
                    OrderSummery = new UserOrderHeader(),
                    CartUserId = userId,
                };

                if (currentUser != null)
                {
                    // assigning the user's info from database as default address
                    summeryViewModel.OrderSummery.DeliveryStreetAddress = currentUser.Address!;
                    summeryViewModel.OrderSummery.City = currentUser.City!;
                    summeryViewModel.OrderSummery.State = currentUser.State!;
                    summeryViewModel.OrderSummery.PostalCode = currentUser.PostalCode!;
                    summeryViewModel.OrderSummery.Name = currentUser.FirstName + " " + currentUser.LastName;
                }
                var count = _dbContext.UserCarts.Where(x => userId!.Contains(userId)).Count();
                HttpContext.Session.SetInt32(CartCount.sessionCount, count);
                return View(summeryViewModel);
            }
            return View();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Summery(SummeryViewModel summeryFromViewModel)
        {
            var claim = _signInManager.IsSignedIn(User);
            if (claim)
            {
                var userId = _userManager.GetUserId(User);
                var currentUser = _dbContext.ApplicationUsers.FirstOrDefault(x => x.Id == userId);
                SummeryViewModel summeryViewModel = new SummeryViewModel()
                {
                    UserCartList = _dbContext.UserCarts
                        .Include(x => x.Product)
                        .Where(x => x.UserId.Contains(userId!)).ToList(),
                    OrderSummery = new UserOrderHeader(),
                };

                if (currentUser != null)
                {
                    // assigning the user's info from database as default address
                    summeryViewModel.OrderSummery.Name = summeryFromViewModel.OrderSummery!.Name;
                    summeryViewModel.OrderSummery.DeliveryStreetAddress = summeryFromViewModel.OrderSummery.DeliveryStreetAddress;
                    summeryViewModel.OrderSummery.City = summeryFromViewModel.OrderSummery.City;
                    summeryViewModel.OrderSummery.State = summeryFromViewModel.OrderSummery.State;
                    summeryViewModel.OrderSummery.PostalCode = summeryFromViewModel.OrderSummery.PostalCode;
                    summeryViewModel.OrderSummery.PhoneNumber = summeryFromViewModel.OrderSummery.PhoneNumber;
                    summeryViewModel.OrderSummery.DateOfOrder = DateTime.Now;
                    summeryViewModel.OrderSummery.OrderStatus = "Pending";
                    summeryViewModel.OrderSummery.PaymentStatus = "Not Paid";
                    summeryViewModel.OrderSummery.UserId = summeryFromViewModel.CartUserId;
                    summeryViewModel.OrderSummery.TotalOrderAmount = summeryFromViewModel.OrderSummery.TotalOrderAmount;

                    await _dbContext.AddAsync(summeryViewModel.OrderSummery);
                    await _dbContext.SaveChangesAsync();
                }

                if (summeryFromViewModel.OrderSummery.TotalOrderAmount > 0)
                {
                    var options = new SessionCreateOptions()
                    {
                        SuccessUrl = "http://localhost:5262/Order/OrderSuccess/"+summeryViewModel.OrderSummery.Id,
                        CancelUrl = "http://localhost:5262/Order/OrderCancele/"+summeryViewModel.OrderSummery.Id,
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                    };
                    foreach (var item in summeryViewModel.UserCartList)
                    {
                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)item.Product.Price * 100,
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Product.ProductName,
                                    Description = item.Product.Description,
                                }
                            },
                            Quantity = item.Quantity,
                        };
                        options.LineItems.Add(sessionLineItem);
                    }

                    var service = new SessionService();
                    Session session = service.Create(options);
                    var loadTheOrder = _dbContext.OrderHeaders
                        .FirstOrDefault(x => x.Id == summeryViewModel.OrderSummery.Id);
                    loadTheOrder!.StripeSessionId = session.Id;
                    loadTheOrder.StripePaymentIntendId = session.PaymentIntentId;
                    _dbContext.Update(loadTheOrder);
                    _dbContext.SaveChanges();
                    Response.Headers["Location"] = session.Url;
                    return new StatusCodeResult(303);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult OrderCancele(int id)
        {
            var orderProcessCanseled = _dbContext.OrderHeaders.FirstOrDefault(x => x.Id == id);
            _dbContext.OrderHeaders.RemoveRange(orderProcessCanseled!);
            _dbContext.SaveChanges();
            return RedirectToAction("CartIndex", "Cart");
        }

        public IActionResult OrderSuccess(int id)
        {
           
            var userId = _userManager.GetUserId(User);
            var userCartRemove = _dbContext.UserCarts
                .Where(x => x.UserId.Contains(userId!))
                .ToList();
            var orderProcessed = _dbContext.OrderHeaders.FirstOrDefault(x => x.Id == id);
            // update payment status
            if (orderProcessed != null)
            {
                if (orderProcessed.PaymentStatus == UpdateOrderStatus.PaymentStatusNotPaid)
                {
                    var service = new SessionService();
                    Session session = service.Get(orderProcessed.StripeSessionId);
                    if (session.PaymentStatus.ToLower() == UpdateOrderStatus.PaymentStatusPaid.ToLower())
                    {
                        orderProcessed.PaymentStatus = UpdateOrderStatus.PaymentStatusPaid;
                        orderProcessed.PaymentProccessDate = DateTime.Now;
                        orderProcessed.StripePaymentIntendId = session.PaymentIntentId;
                    }    
                }
            }
            // add the item from cart to order details table
            foreach (var list in userCartRemove)
            {
                OrderDetails orderReceived = new OrderDetails()
                {
                    OrderHeaderId = orderProcessed!.Id,
                    ProductId = (int)list.ProductId,
                    Count = list.Quantity,
                };
                ViewBag.OrderId = id;
                _dbContext.OrderDetails.Add(orderReceived);
            }
            // remove item from cart for the current user after successfully completing the payment
            _dbContext.UserCarts.RemoveRange(userCartRemove);
            _dbContext.SaveChanges();
            HttpContext.Session.Clear();

            return View();
        }

        public IActionResult OrderHistory(string? status)
        {
            // var userId = _userManager.GetUserId(User);
            // List<UserOrderHeader> orderList = new List<UserOrderHeader>();
            // if (status != null && status != "All")
            // {
            //     if (User.IsInRole("Admin"))
            //     {
            //         orderList = _dbContext.OrderHeaders.Where(x =>x.OrderStatus == status).ToList();
            //     }
            //     else
            //     {
            //         orderList = _dbContext.OrderHeaders
            //             .Where(x => x.OrderStatus == status && x.UserId == userId).ToList();
            //     }
            // }
            // else
            // {
            //     if (User.IsInRole("Admin"))
            //     {
            //         orderList = _dbContext.OrderHeaders.ToList();
            //     }
            //     else
            //     {
            //         orderList = _dbContext.OrderHeaders.Where(x => x.UserId == userId).ToList();
            //     }
            // }
            return View();
        }
        public IActionResult OrderListAll(string? status)
        {
             var userId = _userManager.GetUserId(User);
            List<UserOrderHeader> orderList = new List<UserOrderHeader>();
            if (status != null && status != "All")
            {
                if (User.IsInRole("Admin"))
                {
                    orderList = _dbContext.OrderHeaders.Where(x =>x.OrderStatus == status).ToList();
                }
                else
                {
                    orderList = _dbContext.OrderHeaders
                        .Where(x => x.OrderStatus == status && x.UserId == userId).ToList();
                }
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    orderList = _dbContext.OrderHeaders.ToList();
                }
                else
                {
                    orderList = _dbContext.OrderHeaders.Where(x => x.UserId == userId).ToList();
                }
            }
            return Json(new {data = orderList });
        }
        public IActionResult OrderDetails(int id)
        {
            OrderDetailsViewModel = new OrderDetailsViewModel();
            OrderDetailsViewModel.OrderHeader = _dbContext.OrderHeaders
                .FirstOrDefault(x => x.Id == id);
            OrderDetailsViewModel.UserProductList = _dbContext.OrderDetails
                .Include(x => x.Product)
                .Where(x => x.OrderHeaderId == id).ToList();
            return View(OrderDetailsViewModel);
        }

        // update order to InProcess
        public IActionResult InProcess()
        {
            var orderToUpdate = _dbContext.OrderHeaders
                .FirstOrDefault(x => x.Id == OrderDetailsViewModel.OrderHeader!.Id);
            if (orderToUpdate != null)
            {
                orderToUpdate.OrderStatus = UpdateOrderStatus.OrderStatusInProcess;
                _dbContext.OrderHeaders.Update(orderToUpdate);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("OrderDetails" , new {id = OrderDetailsViewModel.OrderHeader!.Id});
        }

        // update order to Shipped
        public IActionResult Shipped()
        {
            var orderToUpdate = _dbContext.OrderHeaders
                .FirstOrDefault(x => x.Id == OrderDetailsViewModel.OrderHeader!.Id);
            if (orderToUpdate != null)
            {
                orderToUpdate.OrderStatus = UpdateOrderStatus.OrderStatusShipped;
                orderToUpdate.Carrier = OrderDetailsViewModel.OrderHeader!.Carrier;
                orderToUpdate.TrackingNumber = OrderDetailsViewModel.OrderHeader.TrackingNumber;
                orderToUpdate.DateOfShipped = DateTime.Now;
                _dbContext.OrderHeaders.Update(orderToUpdate);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("OrderDetails", new { id = OrderDetailsViewModel.OrderHeader!.Id });
        }

        // update order to Delevered
        public IActionResult Delevered()
        {
            var orderToUpdate = _dbContext.OrderHeaders
                .FirstOrDefault(x => x.Id == OrderDetailsViewModel.OrderHeader!.Id);
            if (orderToUpdate != null)
            {
                orderToUpdate.OrderStatus = UpdateOrderStatus.OrderStatusCompleted;
                _dbContext.OrderHeaders.Update(orderToUpdate);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("OrderDetails", new { id = OrderDetailsViewModel.OrderHeader!.Id });
        }

        // update order to Canceled
        public async Task<IActionResult> Canceled(int id)
        {
            var orderToUpdate = _dbContext.OrderHeaders
                .FirstOrDefault(x => x.Id == id);
            if (orderToUpdate != null)
            {
                if(orderToUpdate.PaymentStatus == UpdateOrderStatus.PaymentStatusPaid)
                {
                    var options = new RefundCreateOptions()
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderToUpdate.StripePaymentIntendId,
                    };
                    var service = new RefundService();
                    Refund refund = service.Create(options);
                    orderToUpdate.OrderStatus = UpdateOrderStatus.OrderStatusCanceled;
                    orderToUpdate.PaymentStatus = UpdateOrderStatus.PaymentStatusRefunded;
                }    
                _dbContext.OrderHeaders.Update(orderToUpdate);
                await _dbContext.SaveChangesAsync();
                return Json(new { success = true, message = "Refund successfully" });
            }
            return Json(new { success = false, message = "Refund Faild" });

            //return RedirectToAction("OrderDetails", new { orderId = orderId });
        }
    }
}
