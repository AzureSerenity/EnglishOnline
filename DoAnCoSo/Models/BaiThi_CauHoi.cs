using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnCoSo.Models
{
    public class BaiThi_CauHoi
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("BaiThi")]
        public int BaiThiId { get; set; }
        public BaiThi BaiThi { get; set; }

        [ForeignKey("CauHoi")]
        public int CauHoiId { get; set; }
        //public string cauHoi {  get; set; }
        public CauHoi CauHoi { get; set; }
        public int Order { get; set; }//Numbering of questions
    }
}
