using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoAnCoSo.Models
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string CurrentRole { get; set; }
        public string SelectedRole { get; set; }
        public List<SelectListItem> Roles { get; set; }
    }
}
