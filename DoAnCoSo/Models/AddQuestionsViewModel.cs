using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DoAnCoSo.Models
{
    public class AddQuestionsViewModel
    {
        public int BaiThiId { get; set; }
        [BindNever]
        public List<CauHoi> Questions { get; set; }
        public List<int> SelectedQuestionIds { get; set; }
    }

}
