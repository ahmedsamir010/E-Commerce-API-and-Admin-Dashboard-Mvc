using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models
{
    public class LoginViewModel
    {
            [EmailAddress]
            public string Email { get; set; }
            public string Password { get; set; }
    }
}
