using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DA.Data;
using DA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Drawing.Printing;
namespace DA.Controllers
{
    public class MathangsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Nhanvien> _passwordHasher;
        public MathangsController(ApplicationDbContext context, IPasswordHasher<Nhanvien> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }
        //public MathangsController(ApplicationDbContext context)
        //      {
        //          _context = context;
        //      }
        public string? Upload(IFormFile file)
        {
            string? uploadFileName = null;
            if (file != null)
            {

                uploadFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var path = $"wwwroot\\images\\products\\{uploadFileName}";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

            }
            return uploadFileName;
        }
        // GET: Mathangs
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            //List<Mathang> mathangs;
            //if (TuKhoa != "" && TuKhoa != null)
            //{
            //    mathangs = _context.Mathangs.Where(p => p.Ten.Contains(TuKhoa) || p.MoTa.Contains(TuKhoa))
            //        .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
            //        .Take(pageSize)  // Lấy số sản phẩm theo pageSize
            //        .ToList(); ;
            //}
            //else
            //{
            //    mathangs = _context.Mathangs
            //        .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
            //        .Take(pageSize)  // Lấy số sản phẩm theo pageSize.ToList();
            //        .ToList();
            //}
            var applicationDbContext = _context.Mathangs.Include(m => m.MaNhNavigation)
                .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                .Take(pageSize);
            //GetData();
            //return View(await applicationDbContext.ToListAsync());
            // Lấy tổng số sản phẩm
            var totalItems = await _context.Mathangs.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            // Gán các giá trị cho ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = totalPages;
            //ViewBag.tukhoa = TuKhoa;
            GetData();
            //return View(mathangs);
            //var applicationDbContext = _context.Mathangs.Include(m => m.MaNhNavigation);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> TimKiemMatHang(string TuKhoa = "", int page = 1, int pageSize = 10)
        {
            List<Mathang> mathangs;
            if (TuKhoa != "" && TuKhoa != null)
            {
                mathangs = _context.Mathangs.Where(p => p.Ten.Contains(TuKhoa) || p.MoTa.Contains(TuKhoa))
                    .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                    .Take(pageSize)  // Lấy số sản phẩm theo pageSize
                    .ToList();
                // Tính tổng số sản phẩm dựa trên kết quả tìm kiếm
                var totalItems = await _context.Mathangs
                    .Where(p => p.Ten.Contains(TuKhoa) || p.MoTa.Contains(TuKhoa))
                    .CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.TotalPage = totalPages;
                ViewBag.tukhoa = TuKhoa;
            }
            else
            {
                mathangs = _context.Mathangs
                    .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                    .Take(pageSize)  // Lấy số sản phẩm theo pageSize.ToList();
                    .ToList();
                // Tính tổng số sản phẩm(không lọc theo từ khóa)
                var totalItems = await _context.Mathangs.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.TotalPage = totalPages;
                ViewBag.tukhoa = TuKhoa;
            }
            //var applicationDbContext = _context.Mathangs.Include(m => m.MaNhNavigation)
            //    .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
            //    .Take(pageSize);
            //GetData();
            //return View(await applicationDbContext.ToListAsync());
            // Lấy tổng số sản phẩm
            //var totalItems = await _context.Hoadons.CountAsync();

            // Tính tổng số trang
            // Gán các giá trị cho ViewBag
            GetData();
            return View(mathangs);
            //var applicationDbContext = _context.Mathangs.Include(m => m.MaNhNavigation);
            //return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> SanPhamBanChay(int page = 1, int pageSize = 10)
        {
            // Lấy tổng số sản phẩm
            var totalItems = await _context.Mathangs.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var sanPhamBanChay = await _context.Mathangs
                .OrderByDescending(sp => sp.LuotMua)
                .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                .Take(pageSize)  // Lấy số sản phẩm theo pageSize
                .ToListAsync();

            // Gán các giá trị cho ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = totalPages;

            GetData();
            return View(sanPhamBanChay);
        }
        public async Task<IActionResult> XemHoaDon(int page = 1, int pageSize = 10)
        {
            //List<Hoadon> hoadons;
            //if (TuKhoa != "" && TuKhoa != null)
            //{
            //    hoadons = _context.Hoadons.Where(p => p.MaKhNavigation.Ten.Contains(TuKhoa) || p.MaKhNavigation.Email.Contains(TuKhoa) || p.MaHd.ToString().Contains(TuKhoa))
            //    .Include(h => h.MaKhNavigation)
            //    .OrderByDescending(m => m.MaHd)
            //    .ToList();
            //}
            //else
            //{
            //    hoadons = _context.Hoadons.Include(h => h.MaKhNavigation)
            //    .OrderByDescending(m => m.MaHd)
            //    .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
            //    .Take(pageSize)  // Lấy số sản phẩm theo pageSize
            //    .ToList();

            //}
            var applicationDbContext = _context.Hoadons.Include(h => h.MaKhNavigation)
                .OrderByDescending(m => m.MaHd)
                .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                .Take(pageSize); ;
            //GetData();
            //return View(await applicationDbContext.ToListAsync());
            // Lấy tổng số sản phẩm
            var totalItems = await _context.Hoadons.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            // Gán các giá trị cho ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = totalPages;
            //ViewBag.tukhoa = TuKhoa;
            GetData();
            return View(await applicationDbContext.ToListAsync());
            //return View(hoadons);
        }
        public async Task<IActionResult> TimKiemHoaDon(string TuKhoa = "", int page = 1, int pageSize = 10)
        {
            List<Hoadon> hoadons;
            if (TuKhoa != "" && TuKhoa != null)
            {
                hoadons = _context.Hoadons.Where(p => p.MaKhNavigation.Ten.Contains(TuKhoa) || p.MaKhNavigation.Email.Contains(TuKhoa) || p.MaHd.ToString().Contains(TuKhoa))
                    .OrderByDescending(m => m.MaHd)
                    .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                    .Take(pageSize)  // Lấy số sản phẩm theo pageSize
                    .ToList();
                // Tính tổng số sản phẩm dựa trên kết quả tìm kiếm
                var totalItems = await _context.Hoadons
                    .Where(p => p.MaKhNavigation.Ten.Contains(TuKhoa) || p.MaKhNavigation.Email.Contains(TuKhoa) || p.MaHd.ToString().Contains(TuKhoa))
                    .CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.TotalPage = totalPages;
                ViewBag.tukhoa = TuKhoa;
            }
            else
            {
                hoadons = await _context.Hoadons
                    .Include(p => p.MaKhNavigation)
                    .OrderByDescending(m => m.MaHd)
                    .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                    .Take(pageSize)  // Lấy số sản phẩm theo pageSize.ToList();
                    .ToListAsync();
                // Tính tổng số sản phẩm(không lọc theo từ khóa)
                var totalItems = await _context.Hoadons.Include(p => p.MaKh).CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.TotalPage = totalPages;
                ViewBag.tukhoa = TuKhoa;
            }
            //var applicationDbContext = _context.Mathangs.Include(m => m.MaNhNavigation)
            //    .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
            //    .Take(pageSize);
            //GetData();
            //return View(await applicationDbContext.ToListAsync());
            // Lấy tổng số sản phẩm
            //var totalItems = await _context.Hoadons.CountAsync();

            // Tính tổng số trang
            // Gán các giá trị cho ViewBag
            GetData();
            return View(hoadons);
            //var applicationDbContext = _context.Mathangs.Include(m => m.MaNhNavigation);
            //return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> XemKhachHang(int page = 1, int pageSize = 10)
        {
            //List<Hoadon> hoadons;
            //if (TuKhoa != "" && TuKhoa != null)
            //{
            //    hoadons = _context.Hoadons.Where(p => p.MaKhNavigation.Ten.Contains(TuKhoa) || p.MaKhNavigation.Email.Contains(TuKhoa) || p.MaHd.ToString().Contains(TuKhoa))
            //    .Include(h => h.MaKhNavigation)
            //    .OrderByDescending(m => m.MaHd)
            //    .ToList();
            //}
            //else
            //{
            //    hoadons = _context.Hoadons.Include(h => h.MaKhNavigation)
            //    .OrderByDescending(m => m.MaHd)
            //    .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
            //    .Take(pageSize)  // Lấy số sản phẩm theo pageSize
            //    .ToList();

            //}
            var applicationDbContext = _context.Khachhangs
                .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                .Take(pageSize); ;
            //GetData();
            //return View(await applicationDbContext.ToListAsync());
            // Lấy tổng số sản phẩm
            var totalItems = await _context.Khachhangs.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            // Gán các giá trị cho ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = totalPages;
            //ViewBag.tukhoa = TuKhoa;
            GetData();
            return View(await applicationDbContext.ToListAsync());
            //return View(hoadons);
        }
        public async Task<IActionResult> TimKiemKhachHang(string TuKhoa = "", int page = 1, int pageSize = 10)
        {
            List<Khachhang> khachhangs;
            if (TuKhoa != "" && TuKhoa != null)
            {
                khachhangs = _context.Khachhangs.Where(p => p.Ten.Contains(TuKhoa) || p.DienThoai.Contains(TuKhoa))
                    .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                    .Take(pageSize)  // Lấy số sản phẩm theo pageSize
                    .ToList();
                // Tính tổng số sản phẩm dựa trên kết quả tìm kiếm
                var totalItems = await _context.Khachhangs
                    .Where(p => p.Ten.Contains(TuKhoa) || p.DienThoai.Contains(TuKhoa))
                    .CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.TotalPage = totalPages;
                ViewBag.tukhoa = TuKhoa;
            }
            else
            {
                khachhangs = _context.Khachhangs
                    .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                    .Take(pageSize)  // Lấy số sản phẩm theo pageSize.ToList();
                    .ToList();
                // Tính tổng số sản phẩm(không lọc theo từ khóa)
                var totalItems = await _context.Khachhangs.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.TotalPage = totalPages;
                ViewBag.tukhoa = TuKhoa;
            }
            //var applicationDbContext = _context.Mathangs.Include(m => m.MaNhNavigation)
            //    .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
            //    .Take(pageSize);
            //GetData();
            //return View(await applicationDbContext.ToListAsync());
            // Lấy tổng số sản phẩm
            //var totalItems = await _context.Hoadons.CountAsync();

            // Tính tổng số trang
            // Gán các giá trị cho ViewBag
            GetData();
            return View(khachhangs);
            //var applicationDbContext = _context.Mathangs.Include(m => m.MaNhNavigation);
            //return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> XemNhanHang(int page = 1, int pageSize = 10)
        {
            //List<Nhanhang> nhanhangs;
            //if (NhanHang != "" && NhanHang != null)
            //{
            //    nhanhangs = _context.Nhanhangs.Where(p => p.Ten.Contains(NhanHang)).ToList();
            //}
            //else
            //{
            //    nhanhangs = _context.Nhanhangs
            //        .Skip((page - 1) * pageSize)
            //        .Take(pageSize)
            //        .ToList();
            //}
            //var applicationDbContext = _context.Mathangs.Include(m => m.MaNhNavigation);
            // Lấy tổng số sản phẩm
            var totalItems = await _context.Nhanhangs.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            // Gán các giá trị cho ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = totalPages;
            GetData();
            //return View(await applicationDbContext.ToListAsync());
            //return View(nhanhangs);
            return View(await _context.Nhanhangs.ToListAsync());
        }
        public async Task<IActionResult> TimKiemNhanHang(string TuKhoa = "", int page = 1, int pageSize = 10)
        {
            List<Nhanhang> nhanhangs;
            if (TuKhoa != "" && TuKhoa != null)
            {
                nhanhangs = _context.Nhanhangs.Where(p => p.Ten.Contains(TuKhoa))
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var totalItems = await _context.Nhanhangs.CountAsync();

                // Tính tổng số trang
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                // Gán các giá trị cho ViewBag
                ViewBag.CurrentPage = page;
                ViewBag.TotalPage = totalPages;
                ViewBag.tukhoa = TuKhoa;
            }
            else
            {
                nhanhangs = _context.Nhanhangs
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                var totalItems = await _context.Nhanhangs.CountAsync();

                // Tính tổng số trang
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                // Gán các giá trị cho ViewBag
                ViewBag.CurrentPage = page;
                ViewBag.TotalPage = totalPages;
                ViewBag.tukhoa = TuKhoa;
            }
            //var applicationDbContext = _context.Mathangs.Include(m => m.MaNhNavigation);
            // Lấy tổng số sản phẩm
            GetData();
            //return View(await applicationDbContext.ToListAsync());
            return View(nhanhangs);
            //return View(await _context.Nhanhangs.ToListAsync());
        }
        // GET: Mathangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mathang = await _context.Mathangs
                .Include(m => m.MaNhNavigation)
                .FirstOrDefaultAsync(m => m.MaMh == id);
            if (mathang == null)
            {
                return NotFound();
            }

            GetData();
            return View(mathang);
        }

        // GET: Mathangs/Create
        public IActionResult Create()
        {
            ViewData["MaNh"] = new SelectList(_context.Nhanhangs, "MaNh", "Ten");
            GetData();
            return View();
        }

        // POST: Mathangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile HinhAnh, [Bind("MaMh,Ten,GiaGoc,GiaBan,SoLuong,MoTa,HinhAnh,MaNh,LuotXem,LuotMua")] Mathang mathang)
        {

            //if (ModelState.IsValid)
            //{
            //    mathang.HinhAnh = Upload(HinhAnh);
            //    _context.Add(mathang);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["MaNh"] = new SelectList(_context.Nhanhangs, "MaNh", "Ten", mathang.MaNh);
            //GetData();
            //return View(mathang);

            mathang.HinhAnh = Upload(HinhAnh);
            _context.Add(mathang);
            await _context.SaveChangesAsync();
            TempData["ThemMH"] = "Thêm sản phầm thành công";
            return RedirectToAction(nameof(Index));
            ViewData["MaNh"] = new SelectList(_context.Nhanhangs, "MaNh", "Ten", mathang.MaNh);
            GetData();
            return View(mathang);
        }

        // GET: Mathangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mathang = await _context.Mathangs.FindAsync(id);
            if (mathang == null)
            {
                return NotFound();
            }
            ViewData["MaNh"] = new SelectList(_context.Nhanhangs, "MaNh", "Ten", mathang.MaNh);
            GetData();
            return View(mathang);
        }

        // POST: Mathangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile HinhAnh, int id, [Bind("MaMh,Ten,GiaGoc,GiaBan,SoLuong,MoTa,HinhAnh,MaNh,LuotXem,LuotMua")] Mathang mathang)
        {
            if (id != mathang.MaMh)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            try
            {
                if (HinhAnh != null && HinhAnh.Length > 0)
                {
                    // Tạo một tên file mới để tránh trùng lặp
                    var fileName = Path.GetFileName(HinhAnh.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", fileName);

                    // Lưu tệp vào thư mục trên server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnh.CopyToAsync(stream);
                    }

                    // Cập nhật đường dẫn tệp vào đối tượng Mathang
                    mathang.HinhAnh = $"\\{fileName}";
                }
                _context.Update(mathang);
                await _context.SaveChangesAsync();
                TempData["SuaMH"] = "Sửa sản phầm thành công";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MathangExists(mathang.MaMh))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
            //}
            ViewData["MaNh"] = new SelectList(_context.Nhanhangs, "MaNh", "Ten", mathang.MaNh);
            GetData();
            return View(mathang);
        }

        // GET: Mathangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mathang = await _context.Mathangs
                .Include(m => m.MaNhNavigation)
                .FirstOrDefaultAsync(m => m.MaMh == id);
            if (mathang == null)
            {
                return NotFound();
            }
            GetData();
            return View(mathang);
        }

        // POST: Mathangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mathang = await _context.Mathangs.FindAsync(id);
            if (mathang != null)
            {
                _context.Mathangs.Remove(mathang);
                TempData["XoaMH"] = "Xóa sản phầm thành công";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MathangExists(int id)
        {
            return _context.Mathangs.Any(e => e.MaMh == id);
        }
        void GetData()
        {
            ViewBag.nhanhang = _context.Nhanhangs.ToList();

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
                TempData["DNNhanVien"] = "Đăng nhập thành công";
                return RedirectToAction(nameof(Index));

            }
            else
            {
                TempData["DNNhanVienTB"] = "Tên đăng nhập hoặc mật khẩu không chính xác!";
            }
            return RedirectToAction(nameof(Login));
        }
        public IActionResult Signout()
        {
            HttpContext.Session.SetString("nhanvien", "");
            return RedirectToAction("Index");
        }
        public IActionResult ThongTinNhanVien()
        {
            GetData();
            return View();
        }




        //Thông tin nhãn hàng
        public async Task<IActionResult> ChiTietNH(int? id)
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

            GetData();
            return View(nhanhang);
        }

        // GET: Nhanhangs/Create
        public IActionResult ThemNH()
        {
            GetData();
            return View();
        }

        // POST: Nhanhangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemNH([Bind("MaNh,Ten")] Nhanhang nhanhang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanhang);
                await _context.SaveChangesAsync();
                TempData["ThemNH"] = "Thêm nhãn hàng thành công";
                return RedirectToAction(nameof(XemNhanHang));
            }
            GetData();
            return View(nhanhang);
        }

        // GET: Nhanhangs/Edit/5
        public async Task<IActionResult> SuaNH(int? id)
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
            GetData();
            return View(nhanhang);
        }

        // POST: Nhanhangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaNH(int id, [Bind("MaNh,Ten")] Nhanhang nhanhang)
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
                    TempData["SuaNH"] = "Sửa nhãn hàng thành công";
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
                return RedirectToAction(nameof(XemNhanHang));
            }
            GetData();
            return View(nhanhang);
        }

        // GET: Nhanhangs/Delete/5
        public async Task<IActionResult> XoaNH(int? id)
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

            GetData();
            return View(nhanhang);
        }

        // POST: Nhanhangs/Delete/5
        [HttpPost, ActionName("XoaNH")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanXoaNH(int id)
        {
            var nhanhang = await _context.Nhanhangs.FindAsync(id);
            if (nhanhang != null)
            {
                _context.Nhanhangs.Remove(nhanhang);
                TempData["XoaNH"] = "Xóa nhãn hàng thành công";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(XemNhanHang));
        }

        private bool NhanhangExists(int id)
        {
            return _context.Nhanhangs.Any(e => e.MaNh == id);
        }





        //Thông tin hóa đơn
        // GET: Hoadons/Details/5
        public async Task<IActionResult> ChiTietHD(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.Hoadons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.Cthoadons)
                .ThenInclude(ct => ct.MaMhNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoadon == null)
            {
                return NotFound();
            }

            GetData();
            return View(hoadon);
        }

        private bool HoadonExists(int id)
        {
            return _context.Hoadons.Any(e => e.MaHd == id);
        }

        public async Task<IActionResult> CapNhatThanhToan(int? id)
        {
            // Tìm khách hàng theo email
            var hoadon = await _context.Hoadons
                .Include(h => h.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);

            hoadon.TrangThai = 1;

            if (ModelState.IsValid)
            {
                _context.Update(hoadon);
                await _context.SaveChangesAsync();
                TempData["CapNhatTrangThai"] = "Hóa đơn đã được thanh toán!";
            }

            // Chuyển hướng về trang thông tin khách hàng
            return RedirectToAction(nameof(XemHoaDon));
        }
        public async Task<IActionResult> CapNhatHuy(int? id)
        {
            // Tìm khách hàng theo email
            var hoadon = await _context.Hoadons
                .Include(h => h.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);

            hoadon.TrangThai = 2;

            if (ModelState.IsValid)
            {
                _context.Update(hoadon);
                await _context.SaveChangesAsync();
                TempData["CapNhatHuy"] = "Hóa đơn đã được hủy!";
            }

            // Chuyển hướng về trang thông tin khách hàng
            return RedirectToAction(nameof(XemHoaDon));
        }

        public async Task<IActionResult> DoanhThuTheoNgay()
        {

            // Lấy tất cả các hóa đơn từ cơ sở dữ liệu
            var hoadons = await _context.Hoadons
                .Where(h => h.TrangThai == 1)  // Chỉ lấy những hóa đơn có ngày lập
                .ToListAsync();

            // Nhóm hóa đơn theo ngày (bỏ qua phần giờ, phút, giây) và tính tổng tiền mỗi ngày
            var doanhThuTheoNgay = hoadons
                .GroupBy(h => h.Ngay.Value.Date)  // Nhóm hóa đơn theo ngày
                .Select(g => new
                {
                    Ngay = g.Key,                      // Ngày của nhóm
                    TongDoanhThu = g.Sum(h => h.TongTien)  // Tính tổng tiền cho mỗi ngày
                })
                .OrderBy(d => d.Ngay)  // Sắp xếp theo ngày
                .ToList();

            var tdt = hoadons.Sum(h => h.TongTien);
            ViewBag.tdt = tdt;

            // Truyền dữ liệu doanh thu và ngày vào view
            var doanhThuModel = doanhThuTheoNgay.Select(d => new
            {
                Ngay = d.Ngay.ToString("dd/MM/yyyy"),
                TongDoanhThu = d.TongDoanhThu
            }).ToList();

            // Trả dữ liệu về View
            ViewBag.DoanhThuData = JsonConvert.SerializeObject(doanhThuModel);

            // Truyền ngày hôm nay vào ViewBag
            ViewBag.NgayHomNay = DateTime.Today.ToString("dd/MM/yyyy");
            // Tính doanh thu của ngày hôm nay
            var ngayHomNay = DateTime.Today;  // Lấy ngày hôm nay (không có giờ, phút, giây)

            var doanhThuHomNay = hoadons
                .Where(h => h.Ngay.Value.Date == ngayHomNay)  // Lọc hóa đơn có ngày lập là hôm nay
                .Sum(h => h.TongTien);  // Tính tổng tiền của các hóa đơn trong ngày hôm nay

            ViewBag.DoanhThuHomNay = doanhThuHomNay;
            // Trả kết quả về view
            GetData();
            return View(doanhThuTheoNgay);
        }




        //Thông tin khách hàng
        public async Task<IActionResult> ChiTietKH(int? id)
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

            GetData();
            return View(khachhang);
        }


        public IActionResult DoiMatKhau()
        {
            GetData();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DoiMatKhau(string email, string matkhaucu, string matkhaumoi)
        {
            var nv = await _context.Nhanviens.FirstOrDefaultAsync(m => m.Email == email);
            var result = _passwordHasher.VerifyHashedPassword(nv, nv.MatKhau, matkhaucu);
            if (nv == null)
            {
                // Nếu không tìm thấy khách hàng, trả về lỗi hoặc redirect về trang thông báo lỗi
                TempData["SuccessMessage"] = "Không tìm thấy nhân viên!";
                GetData();
                return View(); // Trả lại form với thông báo lỗi
            }
            if (string.IsNullOrWhiteSpace(matkhaucu))
            {
                TempData["TrongMatKhauCu"] = "Mật khẩu cũ không được bỏ trống!";
                GetData();
                return View(); // Trả lại form nếu mật khẩu mới trống
            }
            if (result == PasswordVerificationResult.Failed)
            {
                TempData["SaiMatKhau"] = "Mật khẩu cũ không chính xác!";
                GetData();
                return View(); // Trả lại form nếu mật khẩu cũ không đúng
            }
            if (string.IsNullOrWhiteSpace(matkhaumoi))
            {
                TempData["TrongMatKhauMoi"] = "Mật khẩu mới không được bỏ trống!";
                GetData();
                return View(); // Trả lại form nếu mật khẩu mới trống
            }
            // Mã hóa mật khẩu mới
            var hashedPassword = _passwordHasher.HashPassword(nv, matkhaumoi);

            // Cập nhật mật khẩu cho khách hàng
            nv.MatKhau = hashedPassword;

            // Kiểm tra tính hợp lệ của model trước khi lưu
            if (ModelState.IsValid)
            {
                TempData["ThanhCong"] = "Đổi mật khẩu thành công!";
                _context.Update(nv);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
