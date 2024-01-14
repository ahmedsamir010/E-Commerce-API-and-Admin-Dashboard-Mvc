using Microsoft.AspNetCore.Identity;
using Store.Core.Entities.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Repository.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var address = new Address
                {
                    City = "Cairo",
                    Country = "Egypt",
                    FirstName = "Ahmed",
                    LastName = "Samir",
                    Street = "Cairo"
                };

                var user = new AppUser
                {
                    FirstName = "Ahmed",
                    LastName = "Samir",
                    Email = "ahmedsamirsakr50@gmail.com",
                    UserName = "AboSamra",
                    PhoneNumber = "01030094711",
                    Address = address
                };

                address.AppUser = user;

                await userManager.CreateAsync(user, "123456789");
                await AddUserToRoleAsync(userManager, user, "Admin");
            }
        }
        public static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
        }

        public static async Task AddUserToRoleAsync(UserManager<AppUser> userManager, AppUser user, string roleName)
        {
            if (user != null)
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}
