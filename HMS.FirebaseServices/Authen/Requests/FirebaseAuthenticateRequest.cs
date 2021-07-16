using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.FirebaseServices.Authen.Requests
{
    public class FirebaseAuthenticateRequest
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
