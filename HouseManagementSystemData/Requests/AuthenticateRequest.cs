using System.ComponentModel.DataAnnotations;

namespace HMS.Data.Requests
{
    public class AuthenticateRequest
    {
        [Required(ErrorMessage = "UserId is required")]
        public string IdToken { get; set; }
    }
}
