using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BaiThiController : Controller
    {
        private readonly ICauHoiRepository _cauHoiRepository;
        private readonly IBaiThiRepository _baithiRepository;
        private readonly ApplicationDbContext _context;

        public BaiThiController(ICauHoiRepository CauhoiRepository, IBaiThiRepository baiThiRepository, ApplicationDbContext context)
        {
            _cauHoiRepository = CauhoiRepository;
            _baithiRepository = baiThiRepository;
            _context = context;
        }

        [Route("ExamList")]
        public async Task<IActionResult> Index()
        {
            var baiThis = await _baithiRepository.GetAllAsync();
            return View(baiThis);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleRandomize(int id, bool randomize)
        {
            var baiThi = await _baithiRepository.GetByIdAsync(id);
            if (baiThi == null)
            {
                return NotFound();
            }

            baiThi.RandomizeQuestion = randomize;
            await _baithiRepository.UpdateAsync(baiThi);

            return Ok();
        }

        [Route("AddExam")]
        public async Task<IActionResult> Add()
        {
            ViewBag.CauHoiList = new SelectList(await _cauHoiRepository.GetAllAsync(), "Id", "NoiDung");
            return View();
        }

        [Route("AddExam")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(BaiThi baiThi, List<int> selectedCauHois)
        {
            if (ModelState.IsValid)
            {
                if (baiThi.CreateTime == default(DateTime))
                {
                    baiThi.CreateTime = DateTime.Now;
                }

                await _baithiRepository.AddAsync(baiThi);

                foreach (var cauHoiId in selectedCauHois)
                {
                    var baiThiCauHoi = new BaiThi_CauHoi
                    {
                        BaiThiId = baiThi.Id,
                        CauHoiId = cauHoiId,
                        Order = selectedCauHois.IndexOf(cauHoiId) + 1
                    };
                    _context.BaiThi_CauHois.Add(baiThiCauHoi);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CauHoiList = new SelectList(await _cauHoiRepository.GetAllAsync(), "Id", "NoiDung");
            return View(baiThi);
        }

        [Route("DisplayQuestion/{id}")]
        [HttpGet] 
        public async Task<IActionResult> DisplayQuestions(int id) 
        { 
            var baiThi = await _baithiRepository.GetByIdAsync(id); 
            if (baiThi == null) 
            {
                return NotFound(); 
            }

            var allQuestions = await _context.CauHois.Include(c => c.DapAns).ToListAsync();

            var viewModel = new AddQuestionsViewModel 
            { 
                BaiThiId = id, 
                Questions = allQuestions ?? new List<CauHoi>(),
                SelectedQuestionIds = new List<int>() 
            }; 
            return View(viewModel); 
        }

        [HttpPost] 
        public async Task<IActionResult> AddQuestionsToExam(AddQuestionsViewModel model) 
        {
            if (model.SelectedQuestionIds == null || model.SelectedQuestionIds.Count == 0)
            {
                TempData["ErrorMessage"] = "An error occurred while adding a question to the test!";
                return RedirectToAction("DisplayQuestions", new { id = model.BaiThiId });
            }

            var baiThi = await _baithiRepository.GetByIdAsync(model.BaiThiId);
            if (baiThi == null)
            {
                return NotFound();
            }

            foreach (var questionId in model.SelectedQuestionIds)
            {
                // Kiểm tra câu hỏi có phải là mới trong bài thi hay không
                if (!baiThi.BaiThi_CauHois.Any(bc => bc.CauHoiId == questionId))
                {
                    var baiThiCauHoi = new BaiThi_CauHoi
                    {
                        BaiThiId = model.BaiThiId,
                        CauHoiId = questionId,
                        Order = baiThi.BaiThi_CauHois.Count + 1 //stt
                    };

                    _context.BaiThi_CauHois.Add(baiThiCauHoi);
                }
            }

            await _context.SaveChangesAsync();

            var savedBaiThi = await _context.BaiThi_CauHois
                                .Where(bc => bc.BaiThiId == model.BaiThiId)
                                .ToListAsync();

            if (!savedBaiThi.Any())
            {
                TempData["ErrorMessage"] = "\r\nQuestions cannot be added to the test!";
                return RedirectToAction("DisplayQuestions", new { id = model.BaiThiId });
            }

            return RedirectToAction("Index"); 
        }

        [Route("UpdateExam")]
        public async Task<IActionResult> Update(int id)
        {
            var baiThi = await _baithiRepository.GetByIdAsync(id);
            if (baiThi == null)
            {
                return NotFound();
            }
            ViewBag.CauHoiList = new SelectList(await _cauHoiRepository.GetAllAsync(), "Id", "NoiDung");
            return View(baiThi);
        }

        [Route("UpdateExam")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, BaiThi baiThi, List<int> selectedCauHois)
        {
            if (id != baiThi.Id) return NotFound();

            // Cập nhật bài thi vào cơ sở dữ liệu
            await _baithiRepository.UpdateAsync(baiThi);

            // Xóa các câu hỏi cũ liên kết với bài thi
            var existingLinks = _context.BaiThi_CauHois.Where(bc => bc.BaiThiId == id);
            _context.BaiThi_CauHois.RemoveRange(existingLinks);

            if (baiThi.RandomizeQuestion == true) 
            { 
                selectedCauHois = selectedCauHois.OrderBy(x => Guid.NewGuid()).ToList(); 
            }

            // Thêm lại các câu hỏi đã chọn
            foreach (var cauHoiId in selectedCauHois)
            {
                var baiThiCauHoi = new BaiThi_CauHoi
                {
                    BaiThiId = baiThi.Id,
                    CauHoiId = cauHoiId,
                    Order = selectedCauHois.IndexOf(cauHoiId) + 1
                };
                _context.BaiThi_CauHois.Add(baiThiCauHoi);
            }

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            // Quay lại danh sách bài thi
            return RedirectToAction(nameof(Index));
        }

        [Route("DeleteExam")]
        public async Task<IActionResult> Delete(int id)
        {
            var baiThi = await _baithiRepository.GetByIdAsync(id);
            if (baiThi == null)
            {
                return NotFound();
            }
            return View(baiThi);
        }

        [Route("DeleteExam")]
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var baiThi = await _baithiRepository.GetByIdAsync(id);
            if (baiThi != null)
            {
                await _baithiRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
