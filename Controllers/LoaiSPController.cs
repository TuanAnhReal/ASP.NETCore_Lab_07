using System.Linq;
using System.Threading.Tasks;
using ASP.NETCore_Lab_07.Data;
using ASP.NETCore_Lab_07.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCore_Lab_07.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LoaiSPController : Controller
    {
        private readonly AppDbContext _context;

        public LoaiSPController(AppDbContext context)
        {
            _context = context;
        }

        // GET: LoaiSP
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách loại sản phẩm
            var loaiSPs = await _context.LoaiSPs.ToListAsync();
            return View(loaiSPs);
        }

        // GET: LoaiSP/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoaiSP/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoaiSP loaiSP)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaiSP);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiSP);
        }

        // GET: LoaiSP/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var loaiSP = await _context.LoaiSPs.FindAsync(id);
            if (loaiSP == null) return NotFound();

            return View(loaiSP);
        }

        // POST: LoaiSP/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoaiSP loaiSP)
        {
            if (id != loaiSP.MaLoai) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiSP);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiSPExists(loaiSP.MaLoai)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(loaiSP);
        }

        // GET: LoaiSP/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var loaiSP = await _context.LoaiSPs.FirstOrDefaultAsync(m => m.MaLoai == id);
            if (loaiSP == null) return NotFound();

            return View(loaiSP);
        }

        // POST: LoaiSP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiSP = await _context.LoaiSPs.FindAsync(id);
            _context.LoaiSPs.Remove(loaiSP);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaiSPExists(int id)
        {
            return _context.LoaiSPs.Any(e => e.MaLoai == id);
        }
    }
}