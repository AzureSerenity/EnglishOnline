using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnCoSo.Models
{
    public class ChangePassword
    {
        public int id { get; set; }
        [Required, DataType(DataType.Password), Display(Name = "Current Password")]
        public string currentPassword { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "New Password")]
        public string newPassword { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "Confirm new password")]
        [Compare("newPassword", ErrorMessage = "Confirm new password does not match.")]
        public string confirmPassword { get; set; }
    }
}
