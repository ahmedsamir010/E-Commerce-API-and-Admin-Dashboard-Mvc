using Microsoft.AspNetCore.Identity;
using Store.Core.Entities.Identity;
using System.Threading.Tasks;

namespace Store.Core.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser appUser, UserManager<AppUser> userManager);
    }
}
