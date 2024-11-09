using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Aspose.Words.Drawing;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CauHoiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICauHoiRepository _cauHoiRepository;
        private readonly IBaiThiRepository _baiThiRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CauHoiController(ApplicationDbContext context, ICauHoiRepository cauHoiRepository, IBaiThiRepository baiThiRepository, IAnswerRepository answerRepository,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _cauHoiRepository = cauHoiRepository;
            _baiThiRepository = baiThiRepository;
            _answerRepository = answerRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("Admin/QuestionBank")]
        public async Task<IActionResult> Index()
        {
            var cauHois = await _cauHoiRepository.GetAllAsync();

            var cauHoiWithAnswers = await _context.CauHois
                .Include(c => c.DapAns)
                .ToListAsync();
            return View(cauHoiWithAnswers);
        }

        public async Task<IActionResult> AddQuestion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion(CauHoi cauHoiModel, BaiThi_CauHoi baiThi_CauHoi, IFormFile? imageURL, IFormFile? audioURL, string? readingContent, List<string> answers, List<string> isCorrectAnswers, int points)
        {
            //if (ModelState.IsValid)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload");

                if (!Directory.Exists(uploadPath)) 
                { 
                    Directory.CreateDirectory(uploadPath); 
                }

                //Validate Audio File
                if (cauHoiModel.LoaiCauHoi == QuestionType.Listening && audioURL != null)
                {
                    if (IsValidAudioFile(audioURL))
                    {
                        var audioPath = Path.Combine(uploadPath, audioURL.FileName);
                        using (var stream = new FileStream(audioPath, FileMode.Create))
                        {
                            await audioURL.CopyToAsync(stream);
                        }
                        cauHoiModel.FileAudio = audioURL.FileName;
                    }
                    else
                    {
                        ModelState.AddModelError("audioFile", "Invalid audio file (mp3, wav) or size exceeds 5 MB.");
                    }
                }

                // Validate Image File
                if ((cauHoiModel.LoaiCauHoi == QuestionType.Writing || cauHoiModel.LoaiCauHoi == QuestionType.Reading || cauHoiModel.LoaiCauHoi == QuestionType.Listening) && imageURL != null)
                {
                    if (IsValidImageFile(imageURL))
                    {
                        var imagePath = Path.Combine(uploadPath, imageURL.FileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await imageURL.CopyToAsync(stream);
                        }
                        cauHoiModel.FileImage = imageURL.FileName;
                    }
                    else
                    {
                        ModelState.AddModelError("imageFile", "Invalid image file (jpg, png) or size exceeds 2 MB.");
                    }
                }

                if (cauHoiModel.LoaiCauHoi == QuestionType.Reading && !string.IsNullOrEmpty(readingContent))
                {
                    cauHoiModel.TextParagraph = readingContent;
                }

                //cauHoiModel.Point = points;

                await _cauHoiRepository.AddAsync(cauHoiModel);

                //var existingAnswers = await _answerRepository.GetByCauHoiIdAsync(cauHoiModel.Mach);

                if (answers != null && isCorrectAnswers != null && answers.Count == isCorrectAnswers.Count)
                {
                    for (int i = 0; i < answers.Count; i++)
                    {
                        bool isCorrect = bool.TryParse(isCorrectAnswers[i], out bool result) ? result : false;
                        var answer = new Answer
                        {
                            CauHoiId = cauHoiModel.Mach,
                            AnswerText = answers[i],
                            IsAnswer = isCorrect
                        };
                        await _answerRepository.AddAsync(answer);
                        await _context.SaveChangesAsync();
                    }
                }

                else
                { 
                    ModelState.AddModelError("answers", "Answers and correct answers count mismatch."); 
                }

                return RedirectToAction(nameof(Index));
            }
            //ViewBag.Answers = await _answerRepository.GetByCauHoiIdAsync(cauHoiModel.Mach);
            return View(cauHoiModel);
        }

        private bool IsValidAudioFile(IFormFile file)
        {
            var validExtensions = new[] { ".mp3", ".wav" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var isValidExtension = validExtensions.Contains(fileExtension);
            var isValidSize = file.Length <= 5 * 1024 * 1024; // 5 MB limit
            return isValidExtension && isValidSize;
        }

        private bool IsValidImageFile(IFormFile file)
        {
            var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var isValidExtension = validExtensions.Contains(fileExtension);
            var isValidSize = file.Length <= 2 * 1024 * 1024; // 2 MB limit
            return isValidExtension && isValidSize;
        }

        [Route("EditQuestion/{id}")]
        public async Task<IActionResult> EditQuestion(int id)
        {
            var cauHoi = await _cauHoiRepository.GetByIdAsync(id);
            if (cauHoi == null)
            {
                return NotFound();
            }

            // Lấy danh sách các bài thi và chọn bài thi hiện tại
            var baiThis = await _baiThiRepository.GetAllAsync();
            ViewBag.DeThis = new SelectList(baiThis, "Id", "TenBaiThi", cauHoi.BaiThi_CauHois?.FirstOrDefault()?.BaiThiId);

            return View(cauHoi);
        }

        [Route("EditQuestion/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion(CauHoi cauHoi, IFormFile? newImage, IFormFile? newAudio, string? newReadingContent, List<string> answers, List<bool> isCorrectAnswers)
        {
            if (answers.Count != isCorrectAnswers.Count)
            {
                ModelState.AddModelError("answers", "The number of answers and correct answers must be the same.");
            }

            if (ModelState.IsValid)
            {
                string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "upload");

                if (cauHoi.LoaiCauHoi == QuestionType.Listening && newAudio != null)
                {
                    if (IsValidAudioFile(newAudio))
                    {
                        var audioPath = Path.Combine(_webHostEnvironment.WebRootPath, "upload", newAudio.FileName);
                        using (var stream = new FileStream(audioPath, FileMode.Create))
                        {
                            await newAudio.CopyToAsync(stream);
                        }
                        cauHoi.FileAudio = newAudio.FileName;
                    }
                    else
                    {
                        ModelState.AddModelError("newAudio", "Invalid audio file (mp3, wav) or size exceeds 5 MB.");
                    }
                }

                if ((cauHoi.LoaiCauHoi == QuestionType.Listening || cauHoi.LoaiCauHoi == QuestionType.Writing || cauHoi.LoaiCauHoi == QuestionType.Reading) && newImage != null)
                {
                    if (IsValidImageFile(newImage))
                    {
                        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "upload", newImage.FileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await newImage.CopyToAsync(stream);
                        }
                        cauHoi.FileImage = newImage.FileName;
                    }
                    else
                    {
                        ModelState.AddModelError("newImage", "Invalid image file (jpg, png) or size exceeds 2 MB.");
                    }
                }

                if (cauHoi.LoaiCauHoi == QuestionType.Reading && newReadingContent != null)
                {
                    cauHoi.TextParagraph = newReadingContent;
                }

                // Cập nhật câu hỏi vào cơ sở dữ liệu
                await _cauHoiRepository.UpdateAsync(cauHoi);

                // Lấy các đáp án hiện tại của câu hỏi
                var existingAnswers = await _answerRepository.GetByCauHoiIdAsync(cauHoi.Mach);

                // Cập nhật các đáp án
                for (int i = 0; i < answers.Count; i++)
                {
                    var existingAnswer = existingAnswers.FirstOrDefault(a => a.AnswerText == answers[i]);

                    if (existingAnswer != null)
                    {
                        // Nếu đáp án đã thay đổi, cập nhật nó
                        if (existingAnswer.AnswerText != answers[i] || existingAnswer.IsAnswer != isCorrectAnswers[i])
                        {
                            existingAnswer.AnswerText = answers[i];
                            existingAnswer.IsAnswer = isCorrectAnswers[i];
                            await _answerRepository.UpdateAsync(existingAnswer);
                        }
                    }
                    else
                    {
                        // Nếu đáp án không có trong CSDL, thêm mới
                        var newAnswer = new Answer
                        {
                            CauHoiId = cauHoi.Mach,
                            AnswerText = answers[i],
                            IsAnswer = isCorrectAnswers[i]
                        };
                        await _answerRepository.AddAsync(newAnswer);
                    }
                }
                foreach (var existingAnswer in existingAnswers)
                {
                    if (!answers.Contains(existingAnswer.AnswerText))
                    {
                        await _answerRepository.DeleteAsync(existingAnswer.Id); // Cần cài đặt phương thức DeleteAsync nếu chưa có
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Lấy lại danh sách các bài thi khi có lỗi ModelState
            var baiThis = await _baiThiRepository.GetAllAsync();
            ViewBag.DeThis = new SelectList(baiThis, "Id", "TenBaiThi", cauHoi.BaiThi_CauHois?.FirstOrDefault()?.BaiThiId);

            return View(cauHoi);
        }


        [Route("DeleteQuestion/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cauHoi = await _cauHoiRepository.GetByIdAsync(id);
            if (cauHoi == null)
            {
                return NotFound();
            }
            return View(cauHoi);
        }

        [Route("DeleteQuestion/{id}")]
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cauHoi = await _cauHoiRepository.GetByIdAsync(id);
            if (cauHoi == null)
            {
                return NotFound();
            }

            var answers = await _answerRepository.GetByCauHoiIdAsync(cauHoi.Mach);
            _answerRepository.RemoveRange(answers);

            // Xóa chính câu hỏi trong bảng CauHoi
            await _cauHoiRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("Admin/CauHoi/DeleteAjax/{id}")]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var cauHoi = await _cauHoiRepository.GetByIdAsync(id);
            if (cauHoi == null)
            {
                return Json(new { success = false, message = "Question not found." });
            }

            var answers = await _answerRepository.GetByCauHoiIdAsync(cauHoi.Mach);
            if (answers != null && answers.Any())
            {
                _answerRepository.RemoveRange(answers);
            }

            await _cauHoiRepository.DeleteAsync(id);
            return Json(new { success = true });
        }

    }
}
