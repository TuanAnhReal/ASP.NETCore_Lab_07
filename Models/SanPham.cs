using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NETCore_Lab_07.Models
{
    public class SanPham
    {
        [Key]
        public int MaSP { get; set; }

        [Required(ErrorMessage = "Không được để trống tên sản phẩm")]
        public string TenSP { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        public decimal DonGia { get; set; }

        public string Anh { get; set; }

        [Required(ErrorMessage = "Phải chọn loại sản phẩm")]
        public int MaLoai { get; set; }

        [ForeignKey("MaLoai")]
        public LoaiSP LoaiSP { get; set; }
    }
}
