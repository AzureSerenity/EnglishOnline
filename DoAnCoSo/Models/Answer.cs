using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnCoSo.Models
{
    public class Answer
    {
        public int Id { get; set; }
        [Required]
        public string AnswerText { get; set; }
        public bool IsAnswer { get; set; }
        [ForeignKey("CauHoi")]
        public int CauHoiId { get; set; }
        public CauHoi cauHoi { get; set; }
        //public ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
