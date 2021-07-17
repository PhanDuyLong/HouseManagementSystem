using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.FirebaseServices.Authen.Requests
{
    public class FirebaseAuthenticateRequest
    {
        [Required(ErrorMessage = "UserId is required")]
        public string IdToken { get; set; }
    }
}
