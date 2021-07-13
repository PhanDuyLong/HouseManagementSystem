using System.ComponentModel.DataAnnotations;

namespace HMS.Data.Requests
{
    public class AuthenticateGoogleRequest
    {
        [Required(ErrorMessage = "User Id is required")]
        public string UserId { get; set; }
    }
}
