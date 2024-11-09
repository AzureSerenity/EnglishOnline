//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//namespace DoAnCoSo.Models
//{
//    public class UserAnswer
//    {
//        [Key]
//        public int Id { get; set; }

//        [ForeignKey("ApplicationUser")]
//        public string UserId { get; set; }
//        public ApplicationUser User { get; set; }

//        [ForeignKey("CauHoi")]
//        public int CauHoiId { get; set; }
//        public CauHoi CauHoi { get; set; }

//        [ForeignKey("Answer")]
//        public int AnswerId { get; set; }
//        public Answer Answer { get; set; }

//        [ForeignKey("BaiThi")]
//        public int BaiThiId { get; set; }
//        public BaiThi BaiThi { get; set; }
//    }

//}
