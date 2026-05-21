using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASP.NETCore_Lab_07.Models
{
    public class LoaiSP
    {
        [Key]
        public int MaLoai { get; set; }

        [Required(ErrorMessage = "Không được để trống tên loại sản phẩm")]
        public string TenLoai { get; set; }

        public ICollection<SanPham> SanPhams { get; set; }
    }
}
