using System.ComponentModel.DataAnnotations;

namespace HMS.Data.Requests
{
    public class AuthenticateRequest
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
