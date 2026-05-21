using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETCore_Lab_07.Data;
using ASP.NETCore_Lab_07.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCore_Lab_07.Controllers
{
    [Authorize]
    public class SanPhamController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SanPhamController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5; // Số sản phẩm trên 1 trang
            var applicationDbContext = _context.SanPhams.Include(p => p.LoaiSP);

            var totalProducts = await applicationDbContext.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            ViewBag.CurrentPage = page;

            var products = await applicationDbContext
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(products);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["MaLoai"] = new SelectList(_context.LoaiSPs, "MaLoai", "TenLoai");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamVM model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                if (model.Anh != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Anh.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Anh.CopyToAsync(fileStream);
                    }
                }

                SanPham sp = new SanPham
                {
                    TenSP = model.TenSP,
                    DonGia = model.DonGia,
                    MaLoai = model.MaLoai,
                    Anh = uniqueFileName
                };

                _context.Add(sp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaLoai"] = new SelectList(_context.LoaiSPs, "MaLoai", "TenLoai", model.MaLoai);
            return View(model);
        }
        //giao diện sửa
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            // Chuyển dữ liệu từ Model sang ViewModel để hiển thị lên Form
            var model = new SanPhamVM
            {
                MaSP = sp.MaSP,
                TenSP = sp.TenSP,
                DonGia = sp.DonGia,
                MaLoai = sp.MaLoai,
                AnhCu = sp.Anh // Lưu tên ảnh hiện tại vào thuộc tính AnhCu
            };

            ViewData["MaLoai"] = new SelectList(_context.LoaiSPs, "MaLoai", "TenLoai", model.MaLoai);
            return View(model);
        }

        // POST: SanPham/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SanPhamVM model)
        {
            if (id != model.MaSP) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var sp = await _context.SanPhams.FindAsync(model.MaSP);
                    if (sp == null) return NotFound();

                    sp.TenSP = model.TenSP;
                    sp.DonGia = model.DonGia;
                    sp.MaLoai = model.MaLoai;

                    // Xử lý nếu người dùng upload ảnh mới
                    if (model.Anh != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                        // 1. Xóa ảnh cũ (nếu có)
                        if (!string.IsNullOrEmpty(model.AnhCu))
                        {
                            string oldFilePath = Path.Combine(uploadsFolder, model.AnhCu);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // 2. Upload ảnh mới
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Anh.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Anh.CopyToAsync(fileStream);
                        }

                        // Cập nhật tên ảnh mới vào Database
                        sp.Anh = uniqueFileName;
                    }
                    // Nếu không up ảnh mới -> Entity vẫn giữ nguyên tên ảnh cũ vì ta không ghi đè thuộc tính sp.Anh

                    _context.Update(sp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(model.MaSP)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaLoai"] = new SelectList(_context.LoaiSPs, "MaLoai", "TenLoai", model.MaLoai);
            return View(model);
        }

        // GET: SanPham/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sp = await _context.SanPhams
                .Include(s => s.LoaiSP)
                .FirstOrDefaultAsync(m => m.MaSP == id);

            if (sp == null) return NotFound();

            return View(sp); // Trả về trang xác nhận xóa
        }

        // POST: SanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp != null)
            {
                // Xóa file ảnh trong thư mục wwwroot/images
                if (!string.IsNullOrEmpty(sp.Anh))
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", sp.Anh);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // Xóa dữ liệu trong Database
                _context.SanPhams.Remove(sp);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Hàm hỗ trợ kiểm tra sản phẩm có tồn tại không
        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.MaSP == id);
        }
    }
}
