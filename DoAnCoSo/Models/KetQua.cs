using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DoAnCoSo.Models;

namespace DoAnCoSo.Models
{
    public class KetQua
    {
        [Key]
        public int KetQuaId { get; set; }

        [ForeignKey("BaiThi")]
        public int Madt { get; set; }
        public BaiThi BaiThi { get; set; }

        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
        public ApplicationUser User { get; set; }

        public int SoCauDung { get; set; }

        public double Diem { get; set; }

        public DateTime ThoiDiemHoanThanh { get; set; }
    }
}
