using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DA.Data;
using DA.Models;

namespace DA.Controllers
{
    public class QLKhachhangsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QLKhachhangsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: QLKhachhangs
        public async Task<IActionResult> Index(string TenKH = "")
        {
            List<Khachhang> khachhangs;
            if (TenKH != "" && TenKH != null)
            {
                khachhangs = _context.Khachhangs.Where(p => p.Ten.Contains(TenKH)).ToList();
            }
            else
            {
                khachhangs = _context.Khachhangs.ToList();
            }
            //var applicationDbContext = _context.Mathangs.Include(m => m.MaNhNavigation);
            //return View(await applicationDbContext.ToListAsync());
            return View(khachhangs);
            //return View(await _context.Khachhangs.ToListAsync());
        }

        // GET: QLKhachhangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhang = await _context.Khachhangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachhang == null)
            {
                return NotFound();
            }

            return View(khachhang);
        }

        // GET: QLKhachhangs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QLKhachhangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKh,Ten,DienThoai,Email,MatKhau")] Khachhang khachhang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khachhang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(khachhang);
        }

        // GET: QLKhachhangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhang = await _context.Khachhangs.FindAsync(id);
            if (khachhang == null)
            {
                return NotFound();
            }
            return View(khachhang);
        }

        // POST: QLKhachhangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKh,Ten,DienThoai,Email,MatKhau")] Khachhang khachhang)
        {
            if (id != khachhang.MaKh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khachhang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachhangExists(khachhang.MaKh))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(khachhang);
        }

        // GET: QLKhachhangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhang = await _context.Khachhangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachhang == null)
            {
                return NotFound();
            }

            return View(khachhang);
        }

        // POST: QLKhachhangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khachhang = await _context.Khachhangs.FindAsync(id);
            if (khachhang != null)
            {
                _context.Khachhangs.Remove(khachhang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachhangExists(int id)
        {
            return _context.Khachhangs.Any(e => e.MaKh == id);
        }
    }
}
