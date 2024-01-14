using System.ComponentModel.DataAnnotations;

namespace Store.API.Dtos
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        [MinLength(8)]
        public string NewPassword { get; set; }
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
}
