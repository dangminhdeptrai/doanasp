using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DA.Data;
using DA.Models;
using Microsoft.AspNetCore.Identity;

namespace DA.Controllers
{
    public class NhanviensController : Controller
    {
        private readonly ApplicationDbContext _context;

		private readonly IPasswordHasher<Nhanvien> _passwordHasher;
		public NhanviensController(ApplicationDbContext context, IPasswordHasher<Nhanvien> passwordHasher)
		{
			_context = context;
			_passwordHasher = passwordHasher;
		}

        // GET: Nhanviens
        public async Task<IActionResult> Index()
		{
			GetData();
			return View(await _context.Nhanviens.ToListAsync());
        }

        // GET: Nhanviens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanvien = await _context.Nhanviens
                .FirstOrDefaultAsync(m => m.MaNv == id);
            if (nhanvien == null)
            {
                return NotFound();
            }

			GetData();
			return View(nhanvien);
        }

        // GET: Nhanviens/Create
        public IActionResult Create()
		{
			GetData();
			return View();
        }

        // POST: Nhanviens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNv,Ten,DienThoai,Email,MatKhau")] Nhanvien nhanvien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanvien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
			}
			GetData();
			return View(nhanvien);
        }

        // GET: Nhanviens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanvien = await _context.Nhanviens.FindAsync(id);
            if (nhanvien == null)
            {
                return NotFound();
			}
			GetData();
			return View(nhanvien);
        }

        // POST: Nhanviens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNv,Ten,DienThoai,Email,MatKhau")] Nhanvien nhanvien)
        {
            if (id != nhanvien.MaNv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhanvien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanvienExists(nhanvien.MaNv))
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
			GetData();
			return View(nhanvien);
        }

        // GET: Nhanviens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanvien = await _context.Nhanviens
                .FirstOrDefaultAsync(m => m.MaNv == id);
            if (nhanvien == null)
            {
                return NotFound();
            }

			GetData();
			return View(nhanvien);
        }

        // POST: Nhanviens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nhanvien = await _context.Nhanviens.FindAsync(id);
            if (nhanvien != null)
            {
                _context.Nhanviens.Remove(nhanvien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanvienExists(int id)
        {
            return _context.Nhanviens.Any(e => e.MaNv == id);
        }
		void GetData()
		{
			if (HttpContext.Session.GetString("nhanvien") != "")
			{
				ViewBag.nhanvien = _context.Nhanviens.FirstOrDefault(k => k.Email == HttpContext.Session.GetString("nhanvien"));
			}
		}
		public IActionResult Login()
		{
			GetData();
			return View();
		}
        public IActionResult Register()
        {
            GetData();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string hoten, string dienthoai, string email, string matkhau)
        {
            Nhanvien nv = new Nhanvien();
            nv.Ten = hoten;
            nv.DienThoai = dienthoai;
            nv.Email = email;
            nv.MatKhau = _passwordHasher.HashPassword(nv, matkhau); // mã hóa mk 
            if (ModelState.IsValid)
            {
                _context.Add(nv);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Login));
        }
        [HttpPost]
		public async Task<IActionResult> Login(string email, string matkhau)
		{
			var nv = await _context.Nhanviens.FirstOrDefaultAsync(m => m.Email == email);
			if (nv != null && _passwordHasher.VerifyHashedPassword(nv, nv.MatKhau, matkhau) == PasswordVerificationResult.Success)
			{
				// Đăng nhập thành công, thực hiện các hành động cần thiết
				// Ví dụ: Ghi thông tin người dùng vào Session
				HttpContext.Session.SetString("nhanvien", nv.Email);
				return RedirectToAction(nameof(Index));

			}
			return RedirectToAction(nameof(Login));
		}
        public IActionResult Signout()
        {
            HttpContext.Session.SetString("nhanvien", "");
            return RedirectToAction("Index");
        }
    }
}
