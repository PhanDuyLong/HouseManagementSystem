using System.ComponentModel.DataAnnotations;

namespace HMS.Authen.Requests
{
    public class RegisterAccountRequest
    {
        [Required(ErrorMessage = "User Id is required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        public string Role { get; set; }
    }
}
