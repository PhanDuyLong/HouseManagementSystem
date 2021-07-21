using System.ComponentModel.DataAnnotations;

namespace HMS.Authen.Requests
{
    public class AuthenticateInternalRequest
    {
        public string UserId { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
