using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace ISAdminApp.Models
{
    public class User
    {
        public string Email { get; set; }

        public string  Password { get; set; }

        public string ConfirmPassword { get; set; }

    }
}
