using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers

   
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new ()
            { 
                ShoppingCartList = _unitOfWork.shoppingCart.GetAll(u =>u.ApplicationuserId ==  userId,includeProprties : "Product"),
                OrderHeader = new()
            };

            foreach (var Cart in ShoppingCartVM.ShoppingCartList)
            {
                Cart.Price = GetPriceBasedOnQuantity(Cart);
                ShoppingCartVM.OrderHeader.OrderTotal +=(Cart.Price * Cart.Count);


            }
            return View(ShoppingCartVM);
        }

        public IActionResult summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.shoppingCart.GetAll(u => u.ApplicationuserId == userId, includeProprties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.applicationUser.Get(u=> u.Id == userId);
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


            foreach (var Cart in ShoppingCartVM.ShoppingCartList)
            {
                Cart.Price = GetPriceBasedOnQuantity(Cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (Cart.Price * Cart.Count);


            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
		public IActionResult summaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartVM.ShoppingCartList = _unitOfWork.shoppingCart.GetAll(u => u.ApplicationuserId == userId,
                includeProprties: "Product");
				
			
            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
			Applicationuser applicationUser = _unitOfWork.applicationUser.Get(u =>u.Id == userId);
			

			foreach (var Cart in ShoppingCartVM.ShoppingCartList)
			{
				Cart.Price = GetPriceBasedOnQuantity(Cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (Cart.Price * Cart.Count);


			}

            if(applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // customer 
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
				//company 
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;

			}
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new () {
                    ProductId = cart.ProductId,
                    Price = cart.Price,
                    Count = cart.Count,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();

            }
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
                // customer 
                var domain = "https://localhost:7077/";

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                   
                    SuccessUrl = domain+$"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain+ "Customer/Cart/index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    
                    Mode = "payment",
                };
                foreach(var item  in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }


                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new Stripe.Checkout.SessionService();
                Session session= service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id,session.PaymentIntentId);
                _unitOfWork.Save(); 
                Response.Headers.Add("Location",session.Url);
                return new StatusCodeResult(303);
            }
			return RedirectToAction(nameof(OrderConfirmation),new {id = ShoppingCartVM.OrderHeader.Id});
		}

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u =>u.Id == id ,includeProprties:"ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //this is an order by customer

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    HttpContext.Session.Clear();
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }

            }
            List<ShoppingCart> shoppingCarts = _unitOfWork.shoppingCart
                .GetAll(u => u.ApplicationuserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.shoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);  
        }

		public IActionResult plus(int cartId)
        {
            var CartFroomDb = _unitOfWork.shoppingCart.Get(u => u.Id == cartId);
            CartFroomDb.Count++;
            _unitOfWork.shoppingCart.Update(CartFroomDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));   
        }
        public IActionResult minus(int cartId)
        {
            var CartFroomDb = _unitOfWork.shoppingCart.Get(u => u.Id == cartId);
            if(CartFroomDb.Count <= 1)
            {
                _unitOfWork.shoppingCart.Remove(CartFroomDb);
                _unitOfWork.Save();

                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.shoppingCart.GetAll(u => u.ApplicationuserId == CartFroomDb.ApplicationuserId).Count());


            }
            else
            {
                CartFroomDb.Count--;
                _unitOfWork.shoppingCart.Update(CartFroomDb);
                _unitOfWork.Save();


            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult remove(int cartId)
        {
            var CartFroomDb = _unitOfWork.shoppingCart.Get(u => u.Id == cartId);
            _unitOfWork.shoppingCart.Remove(CartFroomDb);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.shoppingCart.GetAll(u => u.ApplicationuserId == CartFroomDb.ApplicationuserId).Count());

            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if(shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
