using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DoAnCoSo.Models;

namespace DoAnCoSo.Models
{
    public class LichSuLamBai
    {
        [Key]
        public int LichSuLamBaiId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }

        [ForeignKey("KetQua")]
        public int KetQuaId { get; set; }

        public DateTime ThoiDiemLamBai { get; set; }

        public ApplicationUser User { get; set; }
        public KetQua KetQua { get; set; }
    }

}
