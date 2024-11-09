using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
    public class BaiThi
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string TenBaiThi { get; set; }

        [Required, StringLength(50)]
        public string MaDe { get; set; }

        [Range(1, 420)]
        public int ThoiGianLamBai { get; set; }

        [Range(1, 180)]
        public int SoLuongCauHoi { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;

        public string? Mota { get; set; }
        public bool RandomizeQuestion { get; set; }

        //public int solanlambai { get; set; }
        public ICollection<BaiThi_CauHoi> BaiThi_CauHois { get; set; } = new List<BaiThi_CauHoi>();
    }
}
