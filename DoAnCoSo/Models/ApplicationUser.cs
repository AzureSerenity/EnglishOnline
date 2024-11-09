using Microsoft.AspNetCore.Identity;

namespace DoAnCoSo.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? Age { get; set; }
        //public ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
