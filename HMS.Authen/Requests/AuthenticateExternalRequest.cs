using System.ComponentModel.DataAnnotations;

namespace HMS.Authen.Requests
{
    public class AuthenticateExternalRequest
    {
        [Required(ErrorMessage = "Id token is required")]
        public string IdToken { get; set; }

        [Required(ErrorMessage = "Provider is required")]
        public string Provider { get; set; }

        public string Role { get; set; }
    }
}
