using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models;

namespace WebBanSach.Controllers
{
    public class NguoiDungController : Controller
    {

        QuanLyBanSachEntities db = new QuanLyBanSachEntities();
        // GET: /NguoiDung/
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DangKy()
        {

            return View();
        }
        [HttpPost]
       // [ValidateAntiForgeryToken]
      
        public ActionResult DangKy(FormCollection f)
        {
            KhachHang temp = new KhachHang();

            temp.Email = f["email"];
            temp.HoTen = f["hoten"];
            temp.DiaChi = f["diachi"];
            temp.MatKhau = f["password"];
            temp.DienThoai = f["sdt"];
            temp.NgaySinh = null;
            temp.GioiTinh = null;
            temp.TaiKhoan = null;
            if (db.KhachHangs.Any(a => a.Email == temp.Email))
            {
                ViewBag.ThongBao = "Email bị trùng xin nhập email khac";
                return View();
            }
            else if (ModelState.IsValid)
            {
                //Chèn dữ liệu vào bảng khách hàng
                db.KhachHangs.Add(temp);
                //Lưu vào csdl 
                db.SaveChanges();
                ViewBag.ThongBao = "Đăng Ký Thành Công";
                
            }
            return RedirectToAction("Index", "Home");

        }
        [HttpGet]
        public ActionResult DangNhap()
        {

            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            string sTaiKhoan = f["txtTaiKhoan"].ToString();
            string sMatKhau = f["txtMatKhau"].ToString();

            KhachHang kh = db.KhachHangs.SingleOrDefault(n => n.TaiKhoan == sTaiKhoan && n.MatKhau == sMatKhau);
            if (kh != null)
            {
                ViewBag.ThongBao = "Chúc mừng bạn đăng nhập thành công !";
                Session["TaiKhoan"] = kh;
                Session["TenKH"] = kh.HoTen;// cái nàydđể hiển thị tên KH khi đăng nhập thôi
                if (kh.TaiKhoan.Equals("admin"))
                    return RedirectToAction("Index", "QuanLySanPham");
                return RedirectToAction("Index","Home");
            }
            ViewBag.ThongBao = "Tên tài khoản hoặc mật khẩu không đúng!";
            return View();
        }
        public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            Session["TenKH"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult EditInfo()
        {

            return View();
        }
	}
}