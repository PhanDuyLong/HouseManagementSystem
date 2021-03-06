using System.ComponentModel.DataAnnotations;

namespace HMS.Authen.Requests
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "User Id is required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Old Password is required")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New Pasword is required")]
        public string NewPassword { get; set; }
    }
}
