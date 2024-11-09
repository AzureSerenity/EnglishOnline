using DoAnCoSo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] 
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private const int PageSize = 5; // Điều chỉnh số lượng người dùng hiển thị trên mỗi trang nếu muốn

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var users = _userManager.Users.OrderBy(u => u.UserName);

            var totalUsers = await users.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)PageSize);

            var paginatedUsers = await users.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = page;

            return View(paginatedUsers);
        }

        [Route("ChangeRole")]
        [HttpGet]
        public async Task<IActionResult> ChangeRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = userRoles.Contains(r.Name)
            });

            var model = new ChangeRoleViewModel
            {
                UserId = userId,
                CurrentRole = userRoles.FirstOrDefault(), // Giả sử mỗi người dùng chỉ có một role
                Roles = roles.ToList()
            };

            return View(model);
        }

        [Route("ChangeRole")]
        [HttpPost]
        public async Task<IActionResult> ChangeRole(ChangeRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles.ToArray());

            var result = await _userManager.AddToRoleAsync(user, model.SelectedRole);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User '{user.UserName}' role changed to '{model.SelectedRole}'.");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // Nếu có lỗi, hiển thị lại form với thông tin đã nhập
            model.Roles = _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = model.SelectedRole == r.Name
            }).ToList();

            return View(model);
        }

        [Route("UserDetails")]
        public async Task<IActionResult> Details(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserDetailsViewModel
            {
                User = user,
                Roles = roles
            };

            return View(model);
        }
    }
}
