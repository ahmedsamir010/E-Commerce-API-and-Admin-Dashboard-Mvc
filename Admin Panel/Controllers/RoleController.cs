using AdminDashboard.Models; // Add this using directive
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
namespace AdminDashboard.Controllers
{
   [Authorize(Roles = "Admin")] 
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleFormView model)
        {
            if (ModelState.IsValid)
            {
                var roleExists = await roleManager.RoleExistsAsync(model.Name);
                if (!roleExists)
                {
                    var newRole = new IdentityRole(model.Name.Trim());
                    var result = await roleManager.CreateAsync(newRole);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("Name", "Role already exists.");
                }
            }

            var roles = await roleManager.Roles.ToListAsync();
            return View("Index", roles);
        }
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            await roleManager.DeleteAsync(role);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            var mappedRole = new RoleViewModel()
            {
                Name = role.Name,
            };
            return View(mappedRole);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var RoleExists = await roleManager.RoleExistsAsync(roleViewModel.Name);
                if (!RoleExists)
                {
                    var role = await roleManager.FindByIdAsync(roleViewModel.Id);
                    role.Name = roleViewModel.Name;
                    await roleManager.UpdateAsync(role);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Name", "Role already Exists");
                    return View("Index", await roleManager.Roles.ToListAsync());
                }
            }
            return RedirectToAction("Index");
        }

    }
}
