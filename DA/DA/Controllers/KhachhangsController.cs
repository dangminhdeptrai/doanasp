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
using Newtonsoft.Json;
using DA.Data;
using DA.Models;
using System.Drawing.Printing;

namespace DA.Controllers
{
    public class KhachhangsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Khachhang> _passwordHasher;
        public KhachhangsController(ApplicationDbContext context, IPasswordHasher<Khachhang> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: Khachhangs
        public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
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

        public async Task<IActionResult> TimKiemMatHang(string TuKhoa = "", int page = 1, int pageSize = 12)
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
        public async Task<IActionResult> ChiTietMatHang(int? id)
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
            mathang.LuotXem = mathang.LuotXem + 1;
            _context.SaveChanges();
            GetData();
            return View(mathang);
        }
        public async Task<IActionResult> AddToCart(int id)
        {
            var mathang = await _context.Mathangs.FirstOrDefaultAsync(m => m.MaMh == id);
            if (mathang == null)
            {
                return NotFound("Sản phẩm không tồn tại");
            }
            var cart = GetCartItems();
            var item = cart.Find(p => p.MatHang.MaMh == id);
            if (item != null)
            {
                item.SoLuong++;
            }
            else
            {
                TempData["ThemGioHang"] = "Sản phẩm đã được thêm vào giỏ hàng!";
                cart.Add(new CartItem() { MatHang = mathang, SoLuong = 1 });
            }
            SaveCartSession(cart);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult ViewCart()
        {
            GetData();
            return View(GetCartItems());
        }
        private bool MathangExists(int id)
        {
            return _context.Mathangs.Any(e => e.MaMh == id);
        }
        private bool KhachhangTonTai(int id)
        {
            return _context.Khachhangs.Any(e => e.MaKh == id);
        }
        private Khachhang KhachhangExists(string email)
        {
            return _context.Khachhangs.FirstOrDefault(e => e.Email == email);
        }
        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string? jsoncart = session.GetString("shopcart");
            if (jsoncart != null)
            {
                var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
                return cartItems ?? new List<CartItem>();
            }
            return new List<CartItem>();
        }
        void SaveCartSession(List<CartItem> list)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(list);
            session.SetString("shopcart", jsoncart);
        }

        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove("shopcart");
        }
        // Xóa một mặt hàng khỏi giỏ
        public IActionResult RemoveItem(int id)
        {
            var cart = GetCartItems();
            var item = cart.Find(p => p.MatHang.MaMh == id);
            if (item != null)
            {
                cart.Remove(item);
            }
            SaveCartSession(cart);
            return RedirectToAction(nameof(ViewCart));
        }
        // Cập nhật số lượng một mặt hàng trong giỏ
        public IActionResult UpdateItem(int id, int quantity)
        {
            var cart = GetCartItems();
            var item = cart.Find(p => p.MatHang.MaMh == id);
            if (item != null)
            {
                item.SoLuong = quantity;
            }
            SaveCartSession(cart);
            return RedirectToAction(nameof(ViewCart));
        }

        public IActionResult CheckOut()
        {
            GetData();
            return View(GetCartItems());
        }

        // Lập hóa đơn: lưu hóa đơn, lưu chi tiết hóa đơn
        [HttpPost, ActionName("CreateBill")]
        public async Task<IActionResult> CreateBill(string email, string hoten, string dienthoai, string diachi)
        {
            // Xử lý thông tin khách hàng (trường hợp khách mới)
            var kh = new Khachhang();
            if (KhachhangExists(email) == null)
            {
                kh.Email = email;
                kh.Ten = hoten;
                kh.DienThoai = dienthoai;
                _context.Add(kh);
                await _context.SaveChangesAsync();
            }
            else
            {
                kh = KhachhangExists(email);
            }

            var hd = new Hoadon();
            hd.Ngay = DateTime.Now;
            hd.MaKh = kh.MaKh;
            _context.Add(hd);
            await _context.SaveChangesAsync();

            // thêm chi tiết hóa đơn
            var cart = GetCartItems();
            int thanhtien = 0;
            int tongtien = 0;
            foreach (var i in cart)
            {
                var ct = new Cthoadon();
                ct.MaHd = hd.MaHd;
                ct.MaMh = i.MatHang.MaMh;
                thanhtien = i.MatHang.GiaBan * i.SoLuong ?? 1;
                tongtien += thanhtien;
                ct.DonGia = i.MatHang.GiaBan;
                ct.SoLuong = (short)i.SoLuong;
                ct.ThanhTien = thanhtien;
                var mh = _context.Mathangs.FirstOrDefault(m => m.MaMh == i.MatHang.MaMh);
                if (mh != null)
                {
                    mh.LuotMua = mh.LuotMua + i.SoLuong ?? 1;
                    mh.SoLuong = (short)(mh.SoLuong - i.SoLuong ?? 1);
                }
                _context.Add(ct);
            }
            _context.SaveChanges();
            await _context.SaveChangesAsync();

            // cập nhật tổng tiền hóa đơn
            hd.TongTien = tongtien;
            _context.Update(hd);
            await _context.SaveChangesAsync();

            // xóa giỏ hàng
            ClearCart();
            GetData();
            return View(hd);
        }

        void GetData()
        {
            ViewData["slg"] = GetCartItems().Count;
            ViewBag.nhanhang = _context.Nhanhangs.ToList();

            if (HttpContext.Session.GetString("khachhang") != "")
            {
                ViewBag.khachhang = _context.Khachhangs.FirstOrDefault(k => k.Email == HttpContext.Session.GetString("khachhang"));
            }
        }

        public async Task<IActionResult> List(int id, int page = 1, int pageSize = 12)
        {
            var applicationDBcontext = _context.Mathangs.Where(m => m.MaNh == id)
                .Include(m => m.MaNhNavigation)
                .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                .Take(pageSize);
            // Lấy tổng số sản phẩm
            var totalItems = await _context.Mathangs
                .Where(m => m.MaNh == id)  // Lọc theo MaNh
                .CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            // Gán các giá trị cho ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = totalPages;
            // Lấy tên nhà cung cấp từ bảng Nhanhangs
            var nhanHang = await _context.Nhanhangs.FirstOrDefaultAsync(d => d.MaNh == id);
            if (nhanHang != null)
            {
                ViewData["id"] = nhanHang.MaNh;
                ViewData["tennhanhang"] = nhanHang.Ten;  // Truyền tên nhà cung cấp vào ViewData
            }
            else
            {
                ViewData["id"] = nhanHang.MaNh;
                ViewData["tennhanhang"] = "Không tìm thấy nhà cung cấp";  // Nếu không tìm thấy, gán giá trị mặc định
            }
            GetData();
            return View(await applicationDBcontext.ToListAsync());
        }
        public IActionResult Register()
        {
            GetData();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string hoten, string dienthoai, string email, string matkhau)
        {
            Khachhang kh = new Khachhang();
            kh.Ten = hoten;
            kh.DienThoai = dienthoai;
            kh.Email = email;
            if (KhachhangExists(email) != null)
            {
                TempData["DangKyThatBai"] = "Email đã tồn tại!";
                return RedirectToAction(nameof(Register));
            }
            kh.MatKhau = _passwordHasher.HashPassword(kh, matkhau); // mã hóa mk 
            if (ModelState.IsValid)
            {
                TempData["DangKy"] = "Đăng ký thành công!";
                _context.Add(kh);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Login));
        }
        public IActionResult Login()
        {
            GetData();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string matkhau)
        {
            var kh = await _context.Khachhangs.FirstOrDefaultAsync(m => m.Email == email);
            if (kh != null && _passwordHasher.VerifyHashedPassword(kh, kh.MatKhau, matkhau) == PasswordVerificationResult.Success)
            {
                // Đăng nhập thành công, thực hiện các hành động cần thiết
                // Ví dụ: Ghi thông tin người dùng vào Session
                TempData["DangNhapThanhCong"] = "Đăng nhập thành công!";
                HttpContext.Session.SetString("khachhang", kh.Email);
                return RedirectToAction(nameof(ThongTinKhachHang));

            }
            else
            {
                TempData["DNKhachHangTB"] = "Tên đăng nhập hoặc mật khẩu không chính xác!";
            }
            return RedirectToAction(nameof(Login));
        }
        public IActionResult ThongTinKhachHang()
        {
            GetData();
            return View();
        }
        public IActionResult Signout()
        {
            TempData["DangXuatThanhCong"] = "Đăng xuất thành công!";
            HttpContext.Session.SetString("khachhang", "");
            GetData();
            return RedirectToAction("Login");
        }
        // GET: Khachhangs/Details/5
        public IActionResult DoiMatKhau()
        {
            GetData();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DoiMatKhau(string email, string matkhaucu, string matkhaumoi)
        {
            var kh = await _context.Khachhangs.FirstOrDefaultAsync(m => m.Email == email);
            var result = _passwordHasher.VerifyHashedPassword(kh, kh.MatKhau, matkhaucu);
            if (kh == null)
            {
                // Nếu không tìm thấy khách hàng, trả về lỗi hoặc redirect về trang thông báo lỗi
                TempData["SuccessMessage"] = "Không tìm thấy khách hàng!";
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
            var hashedPassword = _passwordHasher.HashPassword(kh, matkhaumoi);

            // Cập nhật mật khẩu cho khách hàng
            kh.MatKhau = hashedPassword;

            // Kiểm tra tính hợp lệ của model trước khi lưu
            if (ModelState.IsValid)
            {
                TempData["ThanhCong"] = "Đổi mật khẩu thành công!";
                _context.Update(kh);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ThongTinKhachHang));
        }

        public IActionResult CapNhatThongTin()
        {
            GetData();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CapNhatThongTin(string email, string hoten, string dienthoai)
        {
            // Tìm khách hàng theo email
            var kh = await _context.Khachhangs.FirstOrDefaultAsync(m => m.Email == email);

            // Kiểm tra xem khách hàng có tồn tại không
            if (kh == null)
            {
                TempData["CapNhatTTThatBai"] = "Không tìm thấy khách hàng với email này!";
                return RedirectToAction(nameof(ThongTinKhachHang));
            }

            kh.Email = email;
            kh.Ten = hoten;
            kh.DienThoai = dienthoai;

            if (ModelState.IsValid)
            {
                _context.Update(kh);
                await _context.SaveChangesAsync();
                TempData["CapNhatTTThanhCong"] = "Đổi thông tin thành công!";
            }

            // Chuyển hướng về trang thông tin khách hàng
            return RedirectToAction(nameof(ThongTinKhachHang));
        }


        public async Task<IActionResult> SanPhamBanChay(int page = 1, int pageSize = 12)
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

        public async Task<IActionResult> GiaCaoThap(int page = 1, int pageSize = 12)
        {
            // Lấy tổng số sản phẩm
            var totalItems = await _context.Mathangs.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var sanPhamGiaCao = await _context.Mathangs
                .OrderByDescending(sp => sp.GiaBan)
                .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                .Take(pageSize)  // Lấy số sản phẩm theo pageSize
                .ToListAsync();

            // Gán các giá trị cho ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = totalPages;

            GetData();
            return View(sanPhamGiaCao);
        }

        public async Task<IActionResult> GiaThapCao(int page = 1, int pageSize = 12)
        {
            // Lấy tổng số sản phẩm
            var totalItems = await _context.Mathangs.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var sanPhamGiaThap = await _context.Mathangs
                .OrderBy(sp => sp.GiaBan)
                .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                .Take(pageSize)  // Lấy số sản phẩm theo pageSize
                .ToListAsync();

            // Gán các giá trị cho ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = totalPages;

            GetData();
            return View(sanPhamGiaThap);
        }

        public async Task<IActionResult> LichSuGiaoDich(int page = 1, int pageSize = 10)
        {
            // Tìm khách hàng theo email
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

            var email = HttpContext.Session.GetString("khachhang");

            var applicationDbContext = _context.Hoadons
                .Where(h => h.MaKhNavigation.Email == email)
                .Include(h => h.MaKhNavigation)  // Bao gồm thông tin khách hàng
                .OrderByDescending(h => h.MaHd);  // Sắp xếp theo MaHd giảm dần

            //GetData();
            //return View(await applicationDbContext.ToListAsync());
            // Lấy tổng số sản phẩm
            var totalItems = await applicationDbContext.CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var hoadonkh = await applicationDbContext
                .Skip((page - 1) * pageSize)  // Bỏ qua các trang trước
                .Take(pageSize)  // Lấy số hóa đơn theo pageSize
                .ToListAsync();
            // Gán các giá trị cho ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = totalPages;
            ViewBag.KhachHangEmail = email;
            //ViewBag.tukhoa = TuKhoa;
            GetData();
            return View(hoadonkh);
            //return View(hoadons);
        }
        public async Task<IActionResult> ChiTietLichSuGiaoDich(int? id)
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
    }
}
