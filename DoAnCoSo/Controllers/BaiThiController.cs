using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace DoAnCoSo.Controllers
{
    public class BaiThiController : Controller
    {
        private readonly ICauHoiRepository _cauHoiRepository;
        private readonly IBaiThiRepository _baiThiRepository;
        private readonly IKetQuaRepository _ketQuaRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILichSuLamBaiRepository _lichSuLamBaiRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BaiThiController(ICauHoiRepository cauHoiRepository, IBaiThiRepository baiThiRepository, ApplicationDbContext context, IKetQuaRepository ketQuaRepository, ILichSuLamBaiRepository lichSuLamBaiRepository, UserManager<ApplicationUser> userManager)
        {
            _cauHoiRepository = cauHoiRepository;
            _baiThiRepository = baiThiRepository;
            _ketQuaRepository = ketQuaRepository;
            _context = context;
            _userManager = userManager;
            _lichSuLamBaiRepository = lichSuLamBaiRepository;
        }

        [Route("Test")]
        public async Task<IActionResult> Index()
        {
            var baiThis = await _baiThiRepository.GetAllAsync();
            return View(baiThis);
        }

        [Route("TakeExam")]
        public async Task<IActionResult> Display(int id)
        {
            if (HttpContext.Session.GetString($"BaiThi_{id}_Completed") == "true")
            {
                TempData["Error"] = "You cannot access a submitted exam.";
                return RedirectToAction("Index");
            }

            var baiThi = await _context.BaiThis
                   .Include(bt => bt.BaiThi_CauHois)
                       .ThenInclude(bc => bc.CauHoi)
                           .ThenInclude(ch => ch.DapAns)  // danh sách đáp án cho từng câu hỏi
                   .FirstOrDefaultAsync(bt => bt.Id == id);

            if (baiThi == null)
            {
                return NotFound();
            }

            var cauHois = baiThi.BaiThi_CauHois.Select(bc => bc.CauHoi).ToList();
            ViewBag.BaiThiId = id;
            ViewBag.ThoiGianThi = baiThi.ThoiGianLamBai;

            return View(cauHois);
        }

        //[Route("ExamResult")]
        //[HttpPost]
        //public async Task<IActionResult> KetQua(int baiThiId, string userId)
        //{
        //    var baiThi = _context.BaiThis
        //                 .Include(b => b.BaiThi_CauHois)
        //                 .ThenInclude(bth => bth.CauHoi)
        //                 .FirstOrDefault(b => b.Id == baiThiId);

        //    if (baiThi == null)
        //    {
        //        return NotFound();
        //    }

            //var userAnswers = _context.UserAnswers
            //              .Where(ua => ua.UserId == userId && ua.BaiThiId == baiThiId)
            //              .ToList();

            //int correctAnswersCount = 0;
            //foreach (var baiThiCauHoi in baiThi.BaiThi_CauHois)
            //{
            //    var cauHoi = baiThiCauHoi.CauHoi;
            //    var correctAnswer = cauHoi.DapAns.FirstOrDefault(a => a.IsAnswer); // Đáp án đúng

            //    // Kiểm tra câu trả lời của người dùng
            //    var userAnswer = userAnswers.FirstOrDefault(ua => ua.CauHoiId == cauHoi.Mach);
            //    if (userAnswer != null && userAnswer.AnswerId == correctAnswer.Id)
            //    {
            //        correctAnswersCount++;
            //    }
            //}

            //double score = baiThi.BaiThi_CauHois.Count > 0 ? (double)correctAnswersCount / baiThi.BaiThi_CauHois.Count * 10 : 0;

            //var ketQua = new KetQua
            //{
            //    Madt = baiThiId,
            //    Id = userId,
            //    SoCauDung = correctAnswersCount,
            //    Diem = score,
            //    ThoiDiemHoanThanh = DateTime.Now
            //};

            //_context.KetQuas.Add(ketQua);
            //_context.SaveChanges();

            //var lichSu = new LichSuLamBai
            //{
            //    Id = userId,
            //    KetQuaId = ketQua.KetQuaId,
            //    ThoiDiemLamBai = DateTime.Now
            //};

            //_context.LichSuLamBais.Add(lichSu);
            //_context.SaveChanges();

            //return View(ketQua);
        //}

        [Route("ExamRecords")]
        public async Task<IActionResult> LichSuLamBai()
        {
            var user = await _userManager.GetUserAsync(User);
            var history = await _context.LichSuLamBais
                                 .Where(l => l.Id == user.Id) 
                                 .OrderByDescending(l => l.ThoiDiemLamBai)
                                 .Include(l => l.KetQua)
                                 .ToListAsync();
            return View(history);
        }
    }
}
