using System.ComponentModel.DataAnnotations;

namespace HMS.Authen.Requests
{
    public class AuthenticateInternalRequest
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
