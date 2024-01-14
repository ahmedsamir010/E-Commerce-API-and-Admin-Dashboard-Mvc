using Microsoft.AspNetCore.Mvc;
using Store.Core.Entities;
using Store.Core;
using Microsoft.AspNetCore.Authorization;

namespace AdminDashboard.Controllers
{
    [Authorize(Roles = "Admin")]

    public class TypeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public TypeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var types = await unitOfWork.Repository<ProductType>().GetAllAsync();
            return View(types);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductType Type)
        {
            if (ModelState.IsValid)
            {
                var existingType = await unitOfWork.Repository<ProductType>().GetFirstOrDefaultAsync(b => b.Name == Type.Name);
                if (existingType != null)
                {
                    ModelState.AddModelError("Name", "A Type with the same name already exists.");
                }
                else
                {
                    await unitOfWork.Repository<ProductType>().AddAsync(Type);
                    await unitOfWork.CompleteAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(Type);
        }
        public async Task<IActionResult> Update(int id)
        {
            var Type = await unitOfWork.Repository<ProductType>().GetByIdAsync(id);
            return View(Type);
        }
        [HttpPost]
        public async Task<IActionResult> Update(ProductType Type)
        {
            if (ModelState.IsValid)
            {
                var existingType = await unitOfWork.Repository<ProductType>().GetFirstOrDefaultAsync(b => b.Name == Type.Name);
                if (existingType != null)
                {
                    ModelState.AddModelError("Name", "A Type with the same name already exists.");
                }
                else
                {
                    unitOfWork.Repository<ProductType>().Update(Type);
                    await unitOfWork.CompleteAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(Type);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var Type = await unitOfWork.Repository<ProductType>().GetByIdAsync(id);
            unitOfWork.Repository<ProductType>().Delete(Type);
            await unitOfWork.CompleteAsync();
            return RedirectToAction("Index");
        }
    }
}
