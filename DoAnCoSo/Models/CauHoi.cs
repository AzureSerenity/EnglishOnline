using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnCoSo.Models
{
    public class CauHoi
    {
        [Key]
        public int Mach { get; set; }
        public string Cauhoi { get; set; }
        [Required]
        public QuestionType LoaiCauHoi { get; set; }
        [Required]
        public string Level { get; set; }

        //Depends on question's type
        public string? FileAudio { get; set; }
        public string? FileImage { get; set; }
        public string? TextParagraph { get; set; }

        [Range(1, 20, ErrorMessage = "Scores must be between 1 and 20.")]
        //public int Point {  get; set; }
        public ICollection<Answer> DapAns { get; set; } = new List<Answer>();
        public ICollection<BaiThi_CauHoi> BaiThi_CauHois { get; set; } = new List<BaiThi_CauHoi>();
        //public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }
}
