using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CauhoiController : Controller
    {

        private readonly ICauHoiRepository _CauhoiRepository;
        private readonly IBaiThiRepository _baithiRepository;
        private readonly ApplicationDbContext _context;
        public CauhoiController(ICauHoiRepository CauhoiRepository, IBaiThiRepository baiThiRepository, ApplicationDbContext context)
        {
            _CauhoiRepository = CauhoiRepository;
            _baithiRepository = baiThiRepository;
            _context = context; 
        }

        public async Task<IActionResult> Index()
        {
            var cauHois = await _CauhoiRepository.GetAllAsync();
            return View(cauHois);
        }

        [Route("Display")]
        public async Task<IActionResult> Display(int id)
        {
            var Cauhoi = await _CauhoiRepository.GetByIdAsync(id);
            if (Cauhoi == null)
            {
                return NotFound();
            }
            return View(Cauhoi);
        }
    }
}
