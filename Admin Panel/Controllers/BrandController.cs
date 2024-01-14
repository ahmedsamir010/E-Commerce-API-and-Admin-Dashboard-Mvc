using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Core;
using Store.Core.Entities;
using Store.Core.Specification;
using Store.Repository;

namespace AdminDashboard.Controllers
{
    [Authorize(Roles = "Admin")]

    public class BrandController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BrandController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();

            return View(brands);
        }
         [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductBrand brand)
        {
            try
            {
                // Check if the brand name already exists
                var existingBrand = await _unitOfWork.Repository<ProductBrand>()
                    .GetEntityWithSpecAsync(new BrandByNameSpecification(brand.Name));

                if (existingBrand != null)
                {
                    ModelState.AddModelError("Name", "Brand name already exists.");
                    return View("Index", await _unitOfWork.Repository<ProductBrand>().GetAllAsync());
                }

                await _unitOfWork.Repository<ProductBrand>().AddAsync(brand);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("Index");


            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the brand.");
                return View(brand);
            }
        }




        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(id);

            if (brand == null)
            {
                return NotFound(); // Return 404 Not Found if the brand doesn't exist
            }

            _unitOfWork.Repository<ProductBrand>().Delete(brand);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("Index");
        }


    }
}
