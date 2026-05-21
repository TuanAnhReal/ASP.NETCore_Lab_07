using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ASP.NETCore_Lab_07.Models
{
    public class SanPhamVM
    {
        public int MaSP { get; set; }

        [Required(ErrorMessage = "Không được để trống tên sản phẩm")]
        public string TenSP { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        public decimal DonGia { get; set; }

        [Required(ErrorMessage = "Phải chọn loại sản phẩm")]
        public int MaLoai { get; set; }

        public IFormFile Anh { get; set; } // Dùng để nhận file upload từ Form
        public string AnhCu { get; set; } // Giữ lại ảnh cũ khi Edit
    }
}
