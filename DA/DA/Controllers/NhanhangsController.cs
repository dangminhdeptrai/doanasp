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
    public class NhanhangsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NhanhangsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Nhanhangs
        public async Task<IActionResult> Index(string NhanHang = "")
        {
            List<Nhanhang> nhanhangs;
            if (NhanHang != "" && NhanHang != null)
            {
                nhanhangs = _context.Nhanhangs.Where(p => p.Ten.Contains(NhanHang)).ToList();
            }
            else
            {
                nhanhangs = _context.Nhanhangs.ToList();
            }
            //var applicationDbContext = _context.Mathangs.Include(m => m.MaNhNavigation);
            //GetData();
            //return View(await applicationDbContext.ToListAsync());
            return View(nhanhangs);
            //return View(await _context.Nhanhangs.ToListAsync());
        }

        // GET: Nhanhangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanhang = await _context.Nhanhangs
                .FirstOrDefaultAsync(m => m.MaNh == id);
            if (nhanhang == null)
            {
                return NotFound();
            }

            return View(nhanhang);
        }

        // GET: Nhanhangs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Nhanhangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNh,Ten")] Nhanhang nhanhang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanhang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nhanhang);
        }

        // GET: Nhanhangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanhang = await _context.Nhanhangs.FindAsync(id);
            if (nhanhang == null)
            {
                return NotFound();
            }
            return View(nhanhang);
        }

        // POST: Nhanhangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNh,Ten")] Nhanhang nhanhang)
        {
            if (id != nhanhang.MaNh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhanhang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanhangExists(nhanhang.MaNh))
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
            return View(nhanhang);
        }

        // GET: Nhanhangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanhang = await _context.Nhanhangs
                .FirstOrDefaultAsync(m => m.MaNh == id);
            if (nhanhang == null)
            {
                return NotFound();
            }

            return View(nhanhang);
        }

        // POST: Nhanhangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nhanhang = await _context.Nhanhangs.FindAsync(id);
            if (nhanhang != null)
            {
                _context.Nhanhangs.Remove(nhanhang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanhangExists(int id)
        {
            return _context.Nhanhangs.Any(e => e.MaNh == id);
        }
    }
}
