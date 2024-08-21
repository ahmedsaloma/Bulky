using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.shoppingCart.GetAll(u => u.ApplicationuserId == claim.Value).Count());
            }
            IEnumerable<Product> productsList = _unitOfWork.Product.GetAll(includeProprties: "Category");
            return View(productsList);
        }
        public IActionResult Details(int Id)
        {
            ShoppingCart cart = new()
            {
                
                Product = _unitOfWork.Product.Get(u => u.Id == Id, includeProprties: "Category"),
                Count = 1,
                ProductId = Id
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart ShoppingCart)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCart.ApplicationuserId = userId;

            ShoppingCart CartFromDb = _unitOfWork.shoppingCart.Get(u => u.ApplicationuserId == userId &&
            u.ProductId == ShoppingCart.ProductId);
            if (CartFromDb != null)
            {
                CartFromDb.Count += ShoppingCart.Count;
                _unitOfWork.shoppingCart.Update(CartFromDb);
                _unitOfWork.Save();


            }
            else
            {
                ShoppingCart.Id = 0;

                _unitOfWork.shoppingCart.Add(ShoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.shoppingCart.GetAll(u => u.ApplicationuserId == userId).Count());
            }
            TempData["success"] = "Cart is Updated weeeeeee";

            return RedirectToAction(nameof(Index));


        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
