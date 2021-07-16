﻿using System.ComponentModel.DataAnnotations;

namespace HMS.Data.Requests
{
    public class AuthenticateRequest
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
