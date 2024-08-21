using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{   [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
   

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unit;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unit = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            List<Product> ObjCategoryList = _unit.Product.GetAll(includeProprties:"Category").ToList();
            
            return View(ObjCategoryList);
        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _unit.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),


            });

            //ViewBag.CategoryList = CategoryList;

            ProductVM ProductVM = new() { CategoryList= CategoryList, Product = new Product()};

            if(id == 0 || id == null)
            {
                //create
                return View(ProductVM);

            }
            else
            {
                //update
                ProductVM.Product  = _unit.Product.Get(u => u.Id == id);
                return View(ProductVM);



            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj,IFormFile? file)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
          

            if (ModelState.IsValid)
            {
                if(file != null)
                {
                    string FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath,@"images\product");
                    if(!obj.Product.ImageUrl.IsNullOrEmpty())
                    {
                        string oldImagePath = Path.Combine(wwwRootPath,obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);    

                        }

                    }
                    
                    using (var fileStream = new FileStream(Path.Combine(productPath, FileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);

                    }
                    obj.Product.ImageUrl =@"\images\product\" + FileName;
                        
                }

                if(obj.Product.Id == 0)
                {
                    _unit.Product.Add(obj.Product);


                }
                else
                {
                    _unit.Product.Update(obj.Product);
                }

            
                _unit.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View();

        }

        

        
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {

            Product obj = _unit.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unit.Product.Remove(obj);
            _unit.Save();
            TempData["success"] = "Category Deleted Successfully";

            return RedirectToAction("Index");


        }
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unit.Product.GetAll(includeProprties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Product ProductToBeDeleted = _unit.Product.Get(u => u.Id == id);
            if(ProductToBeDeleted == null)
            {
                return Json(new { success = false, message = "error while deleting" });

            }
            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                ProductToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);

            }
            _unit.Product.Remove(ProductToBeDeleted);
            _unit.Save(); 
                return Json(new { success = true, message = "Delete sccessfuly" });


        }
        #endregion
    }
}
