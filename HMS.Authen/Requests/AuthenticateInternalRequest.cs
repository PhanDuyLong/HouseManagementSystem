using System.ComponentModel.DataAnnotations;

namespace HMS.Authen.Requests
{
    public class AuthenticateInternalRequest
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
    }
}
