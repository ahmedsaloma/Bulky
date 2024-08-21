using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.ViewModels;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unit;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unit = unitOfWork;
           

        }
        public IActionResult Index()
        {
            List<Company> ObjCompanyList = _unit.Company.GetAll().ToList();

            return View(ObjCompanyList);
        }

        public IActionResult Upsert(int? id)
        {
           



            if (id == 0 || id == null)
            {
                //create
                return View(new Company());

            }
            else
            {
                //update
                Company company = _unit.Company.Get(u => u.Id == id);
                return View(company);



            }

        }

        [HttpPost]
        public IActionResult Upsert(Company obj)
        {


            if (ModelState.IsValid)
            {
                

                if (obj.Id == 0)
                {
                    _unit.Company.Add(obj);


                }
                else
                {
                    _unit.Company.Update(obj);
                }


                _unit.Save();
                TempData["success"] = "Company Created Successfully";
                return RedirectToAction("Index");
            }
            return View();

        }




        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {

            Company obj = _unit.Company.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unit.Company.Remove(obj);
            _unit.Save();
            TempData["success"] = "Company Deleted Successfully";

            return RedirectToAction("Index");


        }
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unit.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company CompanyToBeDeleted = _unit.Company.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "error while deleting" });

            }
            
            _unit.Company.Remove(CompanyToBeDeleted);
            _unit.Save();
            return Json(new { success = true, message = "Delete sccessfuly" });


        }
        #endregion
    }
}