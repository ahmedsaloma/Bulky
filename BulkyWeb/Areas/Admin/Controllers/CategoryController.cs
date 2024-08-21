using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _unit;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unit = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> ObjCategoryList = _unit.Category.GetAll().ToList();
            return View(ObjCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {

            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                _unit.Category.Add(obj);
                _unit.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();

            }

            Category categoryFromDb = _unit.Category.Get(u => u.Id == id);
            //Category categoryFromDb2 = _db.Categories.FirstOrDefault(u=>u.Id == id);


            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {


            if (ModelState.IsValid)
            {
                _unit.Category.Update(obj);
                _unit.Save();
                TempData["success"] = "Category Updated Successfully";

                return RedirectToAction("Index");
            }
            return View();

        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();

            }

            Category categoryFromDb = _unit.Category.Get(u => u.Id == id);
            //Category categoryFromDb2 = _db.Categories.FirstOrDefault(u=>u.Id == id);


            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {

            Category obj = _unit.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unit.Category.Remove(obj);
            _unit.Save();
            TempData["success"] = "Category Deleted Successfully";

            return RedirectToAction("Index");


        }
    }
}
